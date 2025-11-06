using System.Collections.Generic;
using System.Linq;
using CustomUtils.Editor.Scripts.Localization.LocalizationSelector;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization.Drawer
{
    internal sealed class LocalizationKeyElement : VisualElement
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly SerializedProperty _guidProperty;

        private readonly DropdownField _keyLabel;
        private readonly ListView _translationList;

        private LocalizationEntry _selectedEntry;
        private List<KeyValuePair<SystemLanguage, string>> _translations;

        private const string LocalizationKeyElementUssClassName = "localization-field";

        internal LocalizationKeyElement(SerializedProperty property, SerializedProperty guidProperty, string label)
        {
            _serializedProperty = property;
            _guidProperty = guidProperty;

            AddToClassList(LocalizationKeyElementUssClassName);

            _keyLabel = new DropdownField(label);
            _keyLabel.AddUnityFileStyles();
            _keyLabel.RegisterInputClick(this, static self => self.ShowKeySelectionWindow());

            Add(_keyLabel);

            _translationList = new ListView
            {
                headerTitle = "Translations",
                showFoldoutHeader = true,
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = static () => new TextField { isReadOnly = true },
                bindItem = BindItem
            };

            _translationList.Q<Foldout>().value = false;

            _keyLabel.value = "[None]";
        }

        internal void Initialize(LocalizationEntry entry)
        {
            _selectedEntry = entry;
            _translations = entry.Translations.ToList();

            _keyLabel.value = entry.Key;

            if (_translations.Count <= 0)
                return;

            var indices = Enumerable.Range(0, _translations.Count).ToList();
            _translationList.itemsSource = indices;
            Add(_translationList);
        }

        private void ShowKeySelectionWindow()
        {
            LocalizationSelectorWindow.ShowWindow(_selectedEntry, OnSelectionChanged);
        }

        private void OnSelectionChanged(LocalizationEntry selectedEntry)
        {
            _guidProperty.stringValue = selectedEntry.GUID;
            _serializedProperty.serializedObject.ApplyModifiedProperties();

            _keyLabel.value = selectedEntry.Key;
            _translations = selectedEntry.Translations.ToList();

            var indices = Enumerable.Range(0, _translations.Count).ToList();
            _translationList.itemsSource = indices;

            _selectedEntry = selectedEntry;

            if (Contains(_translationList) is false)
                Add(_translationList);
        }

        private void BindItem(VisualElement element, int index)
        {
            if (element is not TextField field)
                return;

            field.label = _translations[index].Key.ToString();
            field.value = _translations[index].Value;
        }
    }
}