using CustomUtils.Editor.Scripts.Localization.LocalizationSelector;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization.KeyDrawer
{
    internal sealed class LocalizationKeyElement : VisualElement
    {
        private readonly SerializedProperty _guidProperty;
        private readonly Foldout _translationsContainer;

        private DropdownField _keyDropdown;

        private LocalizationEntry _selectedEntry;

        internal LocalizationKeyElement(SerializedProperty guidProperty, string label)
        {
            _guidProperty = guidProperty;

            _translationsContainer = new Foldout { text = label };

            SetupKeyField(label);

            SetupToggle();

            Add(_translationsContainer);
        }

        internal void UpdateLocalizationKey(LocalizationEntry entry)
        {
            _selectedEntry = entry;

            _keyDropdown.value = entry.Key;

            foreach (var (systemLanguage, localization) in entry.Translations)
            {
                var translation = new TextField
                {
                    isReadOnly = true,
                    label = systemLanguage.ToString(),
                    value = localization
                };

                _translationsContainer.Add(translation);
            }
        }

        private void ShowKeySelectionWindow()
        {
            LocalizationSelectorWindow.ShowWindow(_selectedEntry, OnSelectionChanged);
        }

        private void OnSelectionChanged(LocalizationEntry selectedEntry)
        {
            _guidProperty.stringValue = selectedEntry.GUID;
            _guidProperty.serializedObject.ApplyModifiedProperties();

            UpdateLocalizationKey(selectedEntry);
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
            if (_translationsContainer.TryQ<Foldout>(out var foldout) is false)
                return;

            foldout.value = false;

            if (foldout.TryQ<Toggle>(out var toggle) is false)
                return;

            if (toggle.TryQ<Label>(out var toggleLabel))
                toggleLabel.RemoveFromHierarchy();

            if (toggle.TryQ<VisualElement>(out var toggleInput, className: Toggle.inputUssClassName))
                toggleInput.Add(_keyDropdown);
        }
    }
}