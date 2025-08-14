using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using CustomUtils.Runtime.UI.Theme.ThemeMapping;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(ThemeColorNameAttribute))]
    internal sealed class ThemeColorNamePropertyDrawer : PropertyDrawer
    {
        private ThemeColorDatabase _themeColorDatabase;
        private ThemeHandler _themeHandler;
        private EditorStateControls _editorStateControls;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _themeColorDatabase = _themeColorDatabase ? _themeColorDatabase : ThemeColorDatabase.Instance;
            _themeHandler = _themeHandler ? _themeHandler : ThemeHandler.Instance;

            var colorType = GetColorTypeFromParent(property);
            if (property.propertyType != SerializedPropertyType.String ||
                ValidateColorDatabase(out _, colorType) is false)
                return EditorGUIUtility.singleLineHeight * 1.5f + EditorGUIUtility.standardVerticalSpacing;

            var showPreview = string.IsNullOrEmpty(property.stringValue) is false && property.stringValue != "None";

            return showPreview
                ? EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing
                : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorVisualControls.WarningBox(position, "Use ThemeColorName with string fields only.");
                return;
            }

            var colorType = GetColorTypeFromParent(property);
            if (ValidateColorDatabase(out var colorNames, colorType) is false)
            {
                EditorVisualControls.WarningBox(position, $"No {colorType} colors found in database.");
                return;
            }

            colorNames.Insert(0, "None");
            var currentIndex = colorNames.IndexOf(property.stringValue);

            if (currentIndex == -1)
            {
                currentIndex = 0;
                property.stringValue = string.Empty;
            }

            var popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var newIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, colorNames.ToArray());

            if (newIndex != currentIndex && newIndex >= 0)
            {
                property.stringValue = newIndex == 0 ? string.Empty : colorNames[newIndex];
                property.serializedObject.ApplyModifiedProperties();
            }

            if (string.IsNullOrEmpty(property.stringValue) || property.stringValue == "None")
                return;

            var previewRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            var previewColor = GetPreviewColor(colorType, property.stringValue);
            EditorGUI.ColorField(previewRect, "Preview", previewColor);
        }

        private ColorType GetColorTypeFromParent(SerializedProperty property)
        {
            var parentPath = property.propertyPath[..property.propertyPath.LastIndexOf('.')];
            var parentProperty = property.serializedObject.FindProperty(parentPath);
            var colorTypeProperty = parentProperty?.FindFieldRelative(nameof(ColorMapping.ColorType));

            return colorTypeProperty != null ? (ColorType)colorTypeProperty.enumValueIndex : ColorType.Solid;
        }

        private bool ValidateColorDatabase(out List<string> colorNames, ColorType colorType = ColorType.Solid)
        {
            colorNames = GetColorNamesForType(colorType);
            return colorNames is { Count: > 0 };
        }

        private List<string> GetColorNamesForType(ColorType colorType) =>
            colorType switch
            {
                ColorType.Gradient => _themeColorDatabase.GetColorNames<ThemeGradientColor>(),
                ColorType.Shared => _themeColorDatabase.GetColorNames<ThemeSharedColor>(),
                ColorType.Solid => _themeColorDatabase.GetColorNames<ThemeSolidColor>(),
                _ => new List<string>()
            };

        private Color GetPreviewColor(ColorType colorType, string colorName) =>
            colorType switch
            {
                ColorType.Gradient => GetGradientPreviewColor(colorName),
                ColorType.Shared => GetSharedColor(colorName),
                ColorType.Solid => GetSolidColor(colorName),
                _ => Color.white
            };

        private Color GetGradientPreviewColor(string colorName)
        {
            if (_themeColorDatabase.TryGetColorByName<ThemeGradientColor>(colorName, out var gradientColor) is false)
                return Color.white;

            var gradient = _themeHandler.CurrentThemeType.Value == ThemeType.Light
                ? gradientColor.LightThemeColor
                : gradientColor.DarkThemeColor;

            return gradient?.colorKeys?.Length > 0
                ? gradient.colorKeys[gradient.colorKeys.Length / 2].color
                : Color.white;
        }

        private Color GetSharedColor(string colorName)
            => _themeColorDatabase.TryGetColorByName<ThemeSharedColor>(colorName, out var sharedColor)
                ? sharedColor.Color
                : Color.white;

        private Color GetSolidColor(string colorName)
        {
            if (_themeColorDatabase.TryGetColorByName<ThemeSolidColor>(colorName, out var solidColor))
                return _themeHandler.CurrentThemeType.Value == ThemeType.Light
                    ? solidColor.LightThemeColor
                    : solidColor.DarkThemeColor;

            return Color.white;
        }
    }
}