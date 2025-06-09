using System.Linq;
using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.UI.ImagePixelPerUnit;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI
{
    [CustomPropertyDrawer(typeof(PixelPerUnitPopupAttribute))]
    public class PixelPerUnitDataDrawer : PropertyDrawer
    {
        private SerializedProperty _backgroundTypeNameProperty;
        private SerializedProperty _backgroundTypeCornerRatioProperty;
        private PixelPerUnitDatabase _pixelPerUnitDatabase;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _backgroundTypeNameProperty = property.FindFieldRelative(nameof(PixelPerUnitData.Name));
            _backgroundTypeCornerRatioProperty = property.FindFieldRelative(nameof(PixelPerUnitData.CornerRatio));
            _pixelPerUnitDatabase = PixelPerUnitDatabase.Instance;

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var backgroundTypeNames = _pixelPerUnitDatabase.GetBackgroundTypeNames();

            if (backgroundTypeNames == null || backgroundTypeNames.Count == 0)
            {
                EditorGUI.HelpBox(position, "No background types in database", MessageType.Warning);
                return;
            }

            var currentIndex = backgroundTypeNames.IndexOf(_backgroundTypeNameProperty.stringValue);

            if (currentIndex == -1)
                AssignPixelPerUnitData(_pixelPerUnitDatabase.PixelPerUnitData.First());

            var newIndex = EditorGUI.Popup(position, label.text, currentIndex, backgroundTypeNames.ToArray());

            if (newIndex == currentIndex || newIndex < 0)
                return;

            var selectedData = _pixelPerUnitDatabase.GetPixelPerUnitData(backgroundTypeNames[newIndex]);

            AssignPixelPerUnitData(selectedData);
        }

        private void AssignPixelPerUnitData(PixelPerUnitData pixelPerUnitData)
        {
            _backgroundTypeNameProperty.stringValue = pixelPerUnitData.Name;
            _backgroundTypeCornerRatioProperty.floatValue = pixelPerUnitData.CornerRatio;
        }
    }
}