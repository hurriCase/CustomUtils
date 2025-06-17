using System.Globalization;
using System.Linq;
using CustomUtils.Runtime.UI.ImagePixelPerUnit;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(PixelPerUnitPopupAttribute))]
    public class PixelPerUnitDataDrawer : PropertyDrawer
    {
        private PixelPerUnitDatabase _pixelPerUnitDatabase;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _pixelPerUnitDatabase = PixelPerUnitDatabase.Instance;

            if (_pixelPerUnitDatabase.CornerSizes == null || _pixelPerUnitDatabase.CornerSizes.Count == 0)
                return EditorGUIUtility.singleLineHeight * 1.5f + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_pixelPerUnitDatabase.CornerSizes == null || _pixelPerUnitDatabase.CornerSizes.Count == 0)
            {
                EditorGUI.HelpBox(position, "No pixel per unit types in database", MessageType.Warning);
                return;
            }

            var currentIndex = _pixelPerUnitDatabase.CornerSizes.IndexOf(property.floatValue);

            if (currentIndex == -1)
            {
                property.floatValue = _pixelPerUnitDatabase.CornerSizes.First();
                currentIndex = 0;
            }

            var newIndex = EditorGUI.Popup(position, label.text, currentIndex,
                _pixelPerUnitDatabase.CornerSizes.Select(x => x.ToString(CultureInfo.CurrentCulture))
                    .ToArray());

            if (newIndex == currentIndex || newIndex < 0)
                return;

            property.floatValue = _pixelPerUnitDatabase.CornerSizes[newIndex];
        }
    }
}