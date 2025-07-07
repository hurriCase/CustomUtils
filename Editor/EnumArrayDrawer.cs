using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor
{
    [CustomPropertyDrawer(typeof(EnumArray<,>))]
    public class EnumArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valuesProperty = property.FindPropertyRelative("_values");
            if (valuesProperty == null)
            {
                EditorGUI.LabelField(position, label.text, "EnumArray not initialized");
                return;
            }

            var enumType = fieldInfo.FieldType.GetGenericArguments()[0];
            var enumNames = Enum.GetNames(enumType);

            if (valuesProperty.arraySize != enumNames.Length)
                valuesProperty.arraySize = enumNames.Length;

            EditorGUI.BeginProperty(position, label, property);

            var foldout = property.isExpanded;
            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, foldout, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                var yPos = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                for (var i = 0; i < valuesProperty.arraySize && i < enumNames.Length; i++)
                {
                    var elementProperty = valuesProperty.GetArrayElementAtIndex(i);
                    var elementHeight = EditorGUI.GetPropertyHeight(elementProperty, true);
                    var elementRect = new Rect(position.x, yPos, position.width, elementHeight);

                    EditorGUI.PropertyField(elementRect, elementProperty, new GUIContent(enumNames[i]), true);

                    yPos += elementHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valuesProperty = property.FindPropertyRelative("_values");
            if (valuesProperty == null || property.isExpanded is false)
                return EditorGUIUtility.singleLineHeight;

            var enumType = fieldInfo.FieldType.GetGenericArguments()[0];
            var enumNames = Enum.GetNames(enumType);

            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var i = 0; i < valuesProperty.arraySize && i < enumNames.Length; i++)
            {
                var elementProperty = valuesProperty.GetArrayElementAtIndex(i);
                height += EditorGUI.GetPropertyHeight(elementProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}