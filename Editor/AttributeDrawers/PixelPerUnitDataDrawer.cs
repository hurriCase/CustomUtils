using System.Linq;
using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.UI.ImagePixelPerUnit;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(PixelPerUnitPopupAttribute))]
    public class PixelPerUnitDataDrawer : PropertyDrawer
    {
        private SerializedProperty _pixelPerUnitTypeNameProperty;
        private SerializedProperty _pixelPerUnitTypeCornerRatioProperty;
        private PixelPerUnitDatabase _pixelPerUnitDatabase;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _pixelPerUnitTypeNameProperty = property.FindFieldRelative(nameof(PixelPerUnitData.Name));
            _pixelPerUnitTypeCornerRatioProperty = property.FindFieldRelative(nameof(PixelPerUnitData.CornerRatio));
            _pixelPerUnitDatabase = PixelPerUnitDatabase.Instance;

            if (ValidatePixelPerUnitTypes() is false)
                return EditorGUIUtility.singleLineHeight * 1.5f + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ValidatePixelPerUnitTypes() is false)
            {
                EditorGUI.HelpBox(position, "No pixel per unit types in database", MessageType.Warning);
                return;
            }

            var pixelPerUnitTypeNames = _pixelPerUnitDatabase.GetPixelPerUnitTypeNames();
            var currentIndex = pixelPerUnitTypeNames.IndexOf(_pixelPerUnitTypeNameProperty.stringValue);

            if (currentIndex == -1)
                AssignPixelPerUnitData(_pixelPerUnitDatabase.PixelPerUnitData.First());

            var newIndex = EditorGUI.Popup(position, label.text, currentIndex, pixelPerUnitTypeNames.ToArray());

            if (newIndex == currentIndex || newIndex < 0)
                return;

            var selectedData = _pixelPerUnitDatabase.GetPixelPerUnitData(pixelPerUnitTypeNames[newIndex]);

            AssignPixelPerUnitData(selectedData);
        }

        private bool ValidatePixelPerUnitTypes()
        {
            var pixelPerUnitTypeNames = _pixelPerUnitDatabase.GetPixelPerUnitTypeNames();
            return pixelPerUnitTypeNames != null && pixelPerUnitTypeNames.Count != 0;
        }

        private void AssignPixelPerUnitData(PixelPerUnitData pixelPerUnitData)
        {
            _pixelPerUnitTypeNameProperty.stringValue = pixelPerUnitData.Name;
            _pixelPerUnitTypeCornerRatioProperty.floatValue = pixelPerUnitData.CornerRatio;
        }
    }
}