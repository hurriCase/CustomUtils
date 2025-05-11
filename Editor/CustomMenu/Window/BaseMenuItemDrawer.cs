using CustomUtils.Editor.CustomMenu.MenuItems;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomMenu.Window
{
    [CustomPropertyDrawer(typeof(BaseMenuItem<>), true)]
    internal sealed class BaseMenuItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var menuTargetProp = property.FindFieldRelative(nameof(BaseMenuItem<object>.MenuTarget));
            var menuPathProp = property.FindFieldRelative(nameof(BaseMenuItem<object>.MenuPath));
            var priorityProp = property.FindFieldRelative(nameof(BaseMenuItem<object>.Priority));

            position.height = EditorGUIUtility.singleLineHeight;

            var targetRect = new Rect(position);
            EditorGUI.PropertyField(targetRect, menuTargetProp, new GUIContent(menuTargetProp.displayName));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var menuPathRect = new Rect(position);
            EditorGUI.PropertyField(menuPathRect, menuPathProp, new GUIContent("Menu Path"));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var priorityRect = new Rect(position);
            EditorGUI.PropertyField(priorityRect, priorityProp, new GUIContent("Priority"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;
    }
}