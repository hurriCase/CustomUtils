using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(SelfAttribute))]
    internal class SelfAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.objectReferenceValue)
                AutoAssignComponent(property);

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        private void AutoAssignComponent(SerializedProperty property)
        {
            if (property.TryGetComponent(fieldInfo.FieldType, out var targetComponent))
                return;

            property.objectReferenceValue = targetComponent;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}