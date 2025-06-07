using CustomUtils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not HideIfAttribute hideIfAttribute)
                return;

            if (GetHideIfAttributeResult(hideIfAttribute, property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is not HideIfAttribute hideIfAttribute || GetHideIfAttributeResult(hideIfAttribute, property))
                return EditorGUI.GetPropertyHeight(property, label);

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool GetHideIfAttributeResult(HideIfAttribute hideIfAttribute, SerializedProperty property)
        {
            var sourcePropertyValue = property.serializedObject.FindProperty(hideIfAttribute.ConditionalSourceField);

            if (sourcePropertyValue != null)
                return sourcePropertyValue.boolValue == hideIfAttribute.HideIfSourceTrue;

            return true;
        }
    }
}