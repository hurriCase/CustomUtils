using CustomUtils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.AttributeDrawers.RequiredField
{
    [CustomPropertyDrawer(typeof(RequiredFieldAttribute))]
    internal sealed class RequiredFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);

            if ((property.propertyType != SerializedPropertyType.ObjectReference ||
                 property.objectReferenceValue is not null) &&
                (property.propertyType != SerializedPropertyType.ArraySize ||
                 property.arraySize != 0))
                return;

            GUILayout.Space(30);

            var warningRect = new Rect(
                position.x,
                position.y + position.height + 1.3f,
                position.width,
                position.height + 1.3f);

            EditorGUI.HelpBox(warningRect, "The field is required.", MessageType.Error);
        }
    }
}