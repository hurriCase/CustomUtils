using System;
using System.Collections.Generic;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEditor;
using UnityEngine;
using CustomUtils.Editor.Extensions;

namespace CustomUtils.Editor.EmumArrayPropertyDrawer
{
    [CustomPropertyDrawer(typeof(EnumArray<,>))]
    public class EnumArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumArrayInfo = GetEnumArrayInfo(property);

            EnsureArraySize(enumArrayInfo);
            DrawEnumArrayGUI(position, property, label, enumArrayInfo);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded is false)
                return EditorGUIUtility.singleLineHeight;

            var enumArrayInfo = GetEnumArrayInfo(property);
            return CalculateHeight(enumArrayInfo);
        }

        private EnumArrayInfo GetEnumArrayInfo(SerializedProperty property)
        {
            var enumType = GetEnumType(fieldInfo.FieldType);

            if (enumType == null)
                return default;

            var entriesProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.Entries));
            var enumModeProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.EnumMode));

            var startIndex = (EnumMode)enumModeProperty.enumValueIndex == EnumMode.SkipFirst ? 1 : 0;

            return new EnumArrayInfo(entriesProperty, GetDistinctEnumNames(enumType), startIndex);
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

            for (var i = info.StartIndex; i < info.EntriesProperty.arraySize && i < info.EnumNames.Length; i++)
            {
                var entryProperty = info.EntriesProperty.GetArrayElementAtIndex(i);
                var valueProperty = entryProperty.FindFieldRelative(nameof(Entry<object>.Value));
                var elementHeight = EditorGUI.GetPropertyHeight(valueProperty, true);
                var elementRect = new Rect(position.x, yPosition, position.width, elementHeight);

                EditorGUI.PropertyField(elementRect, valueProperty, new GUIContent(info.EnumNames[i]), true);

                yPosition += elementHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private float CalculateHeight(EnumArrayInfo info)
        {
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var i = info.StartIndex; i < info.EntriesProperty.arraySize && i < info.EnumNames.Length; i++)
            {
                var entryProperty = info.EntriesProperty.GetArrayElementAtIndex(i);
                var valueProperty = entryProperty?.FindFieldRelative(nameof(Entry<object>.Value));
                if (valueProperty != null)
                    height += EditorGUI.GetPropertyHeight(valueProperty, true) +
                              EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        private string[] GetDistinctEnumNames(Type enumType)
        {
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);

            var distinctNames = new List<string>();
            var seenValues = new HashSet<int>();

            for (var i = 0; i < names.Length; i++)
            {
                var intValue = Convert.ToInt32(values.GetValue(i));
                if (seenValues.Add(intValue))
                    distinctNames.Add(names[i]);
            }

            return distinctNames.ToArray();
        }

        private void EnsureArraySize(EnumArrayInfo info)
        {
            if (info.EntriesProperty.arraySize != info.EnumNames.Length)
                info.EntriesProperty.arraySize = info.EnumNames.Length;
        }

        private Type GetEnumType(Type type)
        {
            if (type.IsArray)
                type = type.GetElementType();

            if (type is null)
                return null;

            var genericArgs = type.GetGenericArguments();
            return genericArgs.Length <= 0 ? null : genericArgs[0];
        }
    }
}