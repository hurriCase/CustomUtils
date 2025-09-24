using System.Collections.Generic;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.Databases;
using CustomUtils.Runtime.UI.Theme.ThemeMapping;
using Cysharp.Text;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.UI.Theme
{
    [CustomPropertyDrawer(typeof(ThemeColorNameAttribute))]
    internal sealed class ThemeColorNamePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            property.stringValue.IsValid()
                ? EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing
                : EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var colorType = property.GetPropertyFromParent<ColorType>(nameof(ColorData.ColorType));

            if (colorType == ColorType.None)
                return;

            if (TryGetColorNamesForType(colorType, out var colorNames) is false)
            {
                var message = ZString.Concat("No {0} colors found in database.", colorType);
                EditorVisualControls.WarningBox(position, message);
                return;
            }

            DrawDropdown(colorNames, property, position, label);

            if (property.stringValue.IsValid())
                DrawColorPreview(property, colorType, position);
        }

        private void DrawDropdown(List<string> colorNames, SerializedProperty property, Rect position, GUIContent label)
        {
            var currentIndex = colorNames.IndexOf(property.stringValue);

            if (currentIndex == -1)
            {
                currentIndex = 0;
                property.stringValue = colorNames.Count > 0 ? colorNames[0] : string.Empty;
            }

            var popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var newIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, colorNames.ToArray());

            if (newIndex == currentIndex || newIndex < 0 || newIndex >= colorNames.Count)
                return;

            property.stringValue = colorNames[newIndex];
            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawColorPreview(SerializedProperty property, ColorType colorType, Rect position)
        {
            var previewRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            var colorName = property.stringValue;

            switch (colorType)
            {
                case ColorType.Solid:
                    if (SolidColorDatabase.Instance.TryGetColorByName(ref colorName, out var previewColor))
                        EditorGUI.ColorField(previewRect, "Preview", previewColor);
                    break;

                case ColorType.Gradient:
                    if (GradientColorDatabase.Instance.TryGetColorByName(ref colorName, out var gradient))
                        EditorGUI.GradientField(previewRect, "Preview", gradient);
                    break;
            }
        }

        private bool TryGetColorNamesForType(ColorType colorType, out List<string> colorNames)
        {
            colorNames = colorType switch
            {
                ColorType.Gradient => GradientColorDatabase.Instance.GetColorNames(),
                ColorType.Solid => SolidColorDatabase.Instance.GetColorNames(),
                _ => null
            };

            return colorNames != null && colorNames.Count != 0;
        }
    }
}