using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.EmumArrayPropertyDrawer
{
    [CustomPropertyDrawer(typeof(EnumArray<,>))]
    public class EnumArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var info = GetEnumArrayInfo(property, isNested: false);
            if (!info.IsValid)
            {
                EditorGUI.LabelField(position, label.text, GetErrorMessage(info));
                return;
            }

            EnsureArraySize(info);
            DrawEnumArrayGUI(position, property, label, info);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var info = GetEnumArrayInfo(property, isNested: false);
            if (info.IsValid is false || property.isExpanded is false)
                return EditorGUIUtility.singleLineHeight;

            return CalculateHeight(info);
        }

        private EnumArrayInfo GetEnumArrayInfo(SerializedProperty property, bool isNested)
        {
            var valuesProperty = property.FindPropertyRelative("_values");
            var skipFirstProperty = property.FindPropertyRelative("_skipFirst");
            var enumType = isNested ? GetNestedEnumType() : GetEnumTypeFromField();

            var info = new EnumArrayInfo
            {
                ValuesProperty = valuesProperty,
                EnumType = enumType
            };

            if (enumType == null)
                return info;

            info.EnumNames = Enum.GetNames(enumType);
            info.SkipFirst = skipFirstProperty?.boolValue ?? false;
            info.StartIndex = info.SkipFirst ? 1 : 0;

            return info;
        }

        private void DrawEnumArrayGUI(Rect position, SerializedProperty property, GUIContent label, EnumArrayInfo info)
        {
            EditorGUI.BeginProperty(position, label, property);

            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawEnumElements(position, info);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private void DrawEnumElements(Rect position, EnumArrayInfo info)
        {
            var yPosition = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var i = info.StartIndex; i < info.ValuesProperty.arraySize && i < info.EnumNames.Length; i++)
            {
                var elementProperty = info.ValuesProperty.GetArrayElementAtIndex(i);
                var elementHeight = EditorGUI.GetPropertyHeight(elementProperty, true);
                var elementRect = new Rect(position.x, yPosition, position.width, elementHeight);

                if (IsNestedEnumArray(elementProperty))
                    DrawNestedEnumArray(elementRect, elementProperty, new GUIContent(info.EnumNames[i]));
                else
                    EditorGUI.PropertyField(elementRect, elementProperty, new GUIContent(info.EnumNames[i]), true);

                yPosition += elementHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private void DrawNestedEnumArray(Rect position, SerializedProperty elementProperty, GUIContent label)
        {
            var nestedInfo = GetEnumArrayInfo(elementProperty, isNested: true);
            if (nestedInfo.IsValid is false)
            {
                EditorGUI.LabelField(position, label.text, GetErrorMessage(nestedInfo));
                return;
            }

            EnsureArraySize(nestedInfo);
            DrawEnumArrayGUI(position, elementProperty, label, nestedInfo);
        }

        private float CalculateHeight(EnumArrayInfo info)
        {
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var i = info.StartIndex; i < info.ValuesProperty.arraySize && i < info.EnumNames.Length; i++)
            {
                var elementProperty = info.ValuesProperty.GetArrayElementAtIndex(i);
                if (elementProperty != null)
                    height += EditorGUI.GetPropertyHeight(elementProperty, true) +
                              EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        private void EnsureArraySize(EnumArrayInfo info)
        {
            if (info.ValuesProperty.arraySize != info.EnumNames.Length)
                info.ValuesProperty.arraySize = info.EnumNames.Length;
        }

        private string GetErrorMessage(EnumArrayInfo info)
        {
            if (info.ValuesProperty == null)
                return "EnumArray not initialized";

            return info.EnumType == null ? "Cannot determine enum type" : "Unknown error";
        }

        private Type GetEnumTypeFromField() => GetEnumTypeFromGenericArguments(fieldInfo?.FieldType, argumentIndex: 0);

        private Type GetNestedEnumType()
        {
            if (fieldInfo?.FieldType == null)
                return null;

            var fieldType = fieldInfo.FieldType;
            if (fieldType.IsArray)
                fieldType = fieldType.GetElementType();

            if (fieldType?.IsGenericType != true)
                return null;

            var genericArgs = fieldType.GetGenericArguments();
            if (genericArgs.Length < 2)
                return null;

            var valueType = genericArgs[1];
            return GetEnumTypeFromGenericArguments(valueType, argumentIndex: 0);
        }

        private Type GetEnumTypeFromGenericArguments(Type type, int argumentIndex)
        {
            if (type == null)
                return null;

            if (type.IsArray)
                type = type.GetElementType();

            if (type?.IsGenericType != true)
                return null;

            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length <= argumentIndex)
                return null;

            var enumType = genericArgs[argumentIndex];
            return enumType.IsEnum ? enumType : null;
        }

        private bool IsNestedEnumArray(SerializedProperty elementProperty) =>
            elementProperty.FindPropertyRelative("_values") != null &&
            elementProperty.FindPropertyRelative("_skipFirst") != null;
    }
}