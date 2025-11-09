using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.CustomTypes;
using CustomUtils.Runtime.CustomTypes.Collections;
using CustomUtils.Runtime.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts
{
    [CustomPropertyDrawer(typeof(EnumArray<,>))]
    public class EnumArrayDrawer : PropertyDrawer
    {
        private SerializedProperty _entriesProperty;
        private string[] _enumNames;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (fieldInfo.FieldType.TryGetEnumType(out var enumType) is false)
                return null;

            _enumNames = enumType.GetDistinctEnumNames();
            _entriesProperty = property.FindFieldRelative(nameof(EnumArray<NoneEnum, object>.Entries));

            EnsureSize();

            var container = new Foldout { text = preferredLabel, viewDataKey = preferredLabel };

            CreateEntries(container);

            return container;
        }

        private void CreateEntries(VisualElement container)
        {
            for (var i = 0; i < _entriesProperty.arraySize; i++)
            {
                var entryProperty = _entriesProperty.GetArrayElementAtIndex(i);
                var valueProperty = entryProperty.FindFieldRelative(nameof(Entry<object>.Value));

                var propertyField = new PropertyField(valueProperty, _enumNames[i]);

                container.Add(propertyField);
            }
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
    }
}