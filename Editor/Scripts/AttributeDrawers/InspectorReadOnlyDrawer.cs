using CustomUtils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
    internal class InspectorReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}