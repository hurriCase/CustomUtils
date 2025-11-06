using System.Collections.Generic;
using System.Linq;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.Localization.LocalizationSelector;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization.KeyDrawer
{
    internal sealed class LocalizationKeyElement : VisualElement
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly SerializedProperty _guidProperty;
        private readonly ListView _translationList;

        private DropdownField _keyDropdown;

        private LocalizationEntry _selectedEntry;
        private List<KeyValuePair<SystemLanguage, string>> _translations;

        internal LocalizationKeyElement(SerializedProperty property, SerializedProperty guidProperty, string label)
        {
            _serializedProperty = property;
            _guidProperty = guidProperty;

            _translationList = new NonReorderableListView
            {
                headerTitle = label,
                makeItem = static () => new TextField { isReadOnly = true },
                bindItem = BindItem
            };

            SetupKeyField(label);

            SetupToggle();

            Add(_translationList);
        }

        private void SetupKeyField(string label)
        {
            _keyDropdown = new DropdownField
            {
                label = label,
                value = "[None]",
                style = { marginLeft = 0 }
            };

            if (_keyDropdown.TryQ<Label>(out var fieldLabel))
                fieldLabel.style.marginLeft = 0;

            _keyDropdown.AddUnityFileStyles();
            _keyDropdown.RegisterInputClick(this, static self => self.ShowKeySelectionWindow());
        }

        private void SetupToggle()
        {
            if (_translationList.TryQ<Foldout>(out var foldout) is false)
                return;

            foldout.value = false;

            if (foldout.TryQ<Toggle>(out var toggle) is false)
                return;

            if (toggle.TryQ<Label>(out var toggleLabel))
                toggleLabel.RemoveFromHierarchy();

            if (toggle.TryQ<VisualElement>(out var toggleInput, className: Toggle.inputUssClassName))
                toggleInput.Add(_keyDropdown);
        }

        internal void Initialize(LocalizationEntry entry)
        {
            _selectedEntry = entry;
            _translations = entry.Translations.ToList();

            _keyDropdown.value = entry.Key;

            if (_translations.Count <= 0)
                return;

            var indices = Enumerable.Range(0, _translations.Count).ToList();
            _translationList.itemsSource = indices;
        }

        private void ShowKeySelectionWindow()
        {
            LocalizationSelectorWindow.ShowWindow(_selectedEntry, OnSelectionChanged);
        }

        private void OnSelectionChanged(LocalizationEntry selectedEntry)
        {
            _guidProperty.stringValue = selectedEntry.GUID;
            _serializedProperty.serializedObject.ApplyModifiedProperties();

            _keyDropdown.value = selectedEntry.Key;
            _translations = selectedEntry.Translations.ToList();

            var indices = Enumerable.Range(0, _translations.Count).ToList();
            _translationList.itemsSource = indices;

            _selectedEntry = selectedEntry;
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