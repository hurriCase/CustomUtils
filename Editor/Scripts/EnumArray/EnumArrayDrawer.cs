using System;
using System.Collections.Generic;
using System.Linq;
using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.EnumArray
{
    [CustomPropertyDrawer(typeof(EnumArray<,>))]
    public class EnumArrayDrawer : PropertyDrawer
    {
        private SerializedProperty _entriesProperty;
        private string[] _enumNames;
        private int _startIndex;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (TryGetEnumType(fieldInfo.FieldType, out var enumType) is false)
                return null;

            _enumNames = GetDistinctEnumNames(enumType);

            _entriesProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.Entries));
            var enumModeProperty = property.FindFieldRelative(nameof(EnumArray<EnumMode, object>.EnumMode));
            _startIndex = (EnumMode)enumModeProperty.enumValueIndex == EnumMode.SkipFirst ? 1 : 0;

            EnsureSize();

            return CreateEntries(property);
        }

        private VisualElement CreateEntries(SerializedProperty property)
        {
            var size = _entriesProperty.arraySize - _startIndex;
            var entries = Enumerable.Range(0, size).ToList();
            var entriesList = new ListView(entries)
            {
                showFoldoutHeader = true,
                showBoundCollectionSize = false,
                headerTitle = property.displayName,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = static () => new VisualElement(),
                bindItem = BindItem
            };

            return entriesList;
        }

        private void BindItem(VisualElement container, int index)
        {
            container.Clear();

            var actualIndex = index + _startIndex;

            var entryProperty = _entriesProperty.GetArrayElementAtIndex(actualIndex);
            var valueProperty = entryProperty.FindFieldRelative(nameof(Entry<object>.Value));

            var propertyField = new PropertyField(valueProperty, _enumNames[index]);
            propertyField.BindProperty(valueProperty);

            container.Add(propertyField);
        }

        private void EnsureSize()
        {
            var namesSize = _enumNames.Length;
            if (_entriesProperty.arraySize == namesSize)
                return;

            _entriesProperty.arraySize = namesSize;

            _entriesProperty.serializedObject.ApplyModifiedProperties();
            _entriesProperty.serializedObject.Update();
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

        private bool TryGetEnumType(Type type, out Type enumType)
        {
            enumType = null;
            if (type.IsArray)
                type = type.GetElementType();
            if (type is null)
                return false;

            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length <= 0)
                return false;

            enumType = genericArgs[0];
            return true;
        }
    }
}