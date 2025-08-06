using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEditor;
using UnityEngine;
using CustomUtils.Editor.Extensions;

namespace CustomUtils.Editor.EmumArrayPropertyDrawer
{
    [CustomPropertyDrawer(typeof(EnumArray<,>))]
    public class EnumArrayDrawer : PropertyDrawer, IDisposable
    {
        private static bool _eventsSubscribed;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_eventsSubscribed is false)
            {
                EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
                _eventsSubscribed = true;
            }

            var info = GetEnumArrayInfo(property);
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
            var info = GetEnumArrayInfo(property);
            if (info.IsValid is false || property.isExpanded is false)
                return EditorGUIUtility.singleLineHeight;

            return CalculateHeight(info);
        }

        private EnumArrayInfo GetEnumArrayInfo(SerializedProperty property)
        {
            var entriesProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.Entries));
            var enumModeProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.EnumMode));
            var enumType = GetEnumTypeFromGenericArguments(fieldInfo?.FieldType);

            var info = new EnumArrayInfo
            {
                ValuesProperty = entriesProperty,
                EnumType = enumType
            };

            if (enumType == null)
                return info;

            info.EnumNames = Enum.GetNames(enumType);
            var enumMode = (EnumMode)(enumModeProperty?.enumValueIndex ?? 0);
            info.SkipFirst = enumMode == EnumMode.SkipFirst;
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

        private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            var enumModeProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.EnumMode));
            if (enumModeProperty == null)
                return;

            var currentMode = (EnumMode)enumModeProperty.enumValueIndex;

            menu.AddItem(new GUIContent("Set Mode to Default"),
                currentMode == EnumMode.Default,
                () => {
                    enumModeProperty.enumValueIndex = (int)EnumMode.Default;
                    property.serializedObject.ApplyModifiedProperties();
                });

            menu.AddItem(new GUIContent("Set Mode to SkipFirst"),
                currentMode == EnumMode.SkipFirst,
                () => {
                    enumModeProperty.enumValueIndex = (int)EnumMode.SkipFirst;
                    property.serializedObject.ApplyModifiedProperties();
                });
        }

        private void DrawEnumElements(Rect position, EnumArrayInfo info)
        {
            var yPosition = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var i = info.StartIndex; i < info.ValuesProperty.arraySize && i < info.EnumNames.Length; i++)
            {
                var entryProperty = info.ValuesProperty.GetArrayElementAtIndex(i);
                var valueProperty = entryProperty.FindPropertyRelative("value");
                var elementHeight = EditorGUI.GetPropertyHeight(valueProperty, true);
                var elementRect = new Rect(position.x, yPosition, position.width, elementHeight);

                EditorGUI.PropertyField(elementRect, valueProperty, new GUIContent(info.EnumNames[i]), true);

                yPosition += elementHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private float CalculateHeight(EnumArrayInfo info)
        {
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var i = info.StartIndex; i < info.ValuesProperty.arraySize && i < info.EnumNames.Length; i++)
            {
                var entryProperty = info.ValuesProperty.GetArrayElementAtIndex(i);
                var valueProperty = entryProperty?.FindPropertyRelative("value");
                if (valueProperty != null)
                    height += EditorGUI.GetPropertyHeight(valueProperty, true) +
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

        private Type GetEnumTypeFromGenericArguments(Type type)
        {
            if (type == null)
                return null;

            if (type.IsArray)
                type = type.GetElementType();

            if (type?.IsGenericType != true)
                return null;

            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length <= 0)
                return null;

            var enumType = genericArgs[0];
            return enumType.IsEnum ? enumType : null;
        }

        public void Dispose()
        {
            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
            _eventsSubscribed = false;
        }
    }
}