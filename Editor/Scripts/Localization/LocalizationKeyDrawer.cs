using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Editor.Scripts.Localization.LocalizationSelector;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKey))]
    internal sealed class LocalizationKeyDrawer : PropertyDrawer
    {
        [SerializeField] private VisualTreeAsset _localizationKeyLayout;

        private SerializedProperty _guidProperty;
        private SerializedProperty _translationsProperty;

        private DropdownField _keyLabel;

        private LocalizationEntry _selectedEntry;
        private SerializedProperty _serializedProperty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _serializedProperty = property;

            var container = _localizationKeyLayout.CloneTree();

            _guidProperty = _serializedProperty.FindFieldRelative(nameof(LocalizationKey.GUID));
            _translationsProperty = _serializedProperty.FindFieldRelative(nameof(LocalizationKey.Translations));

            LocalizationRegistry.Instance.Entries.TryGetValue(_guidProperty.stringValue, out _selectedEntry);

            _keyLabel = container.Q<DropdownField>("LocalizationKeyLabel");
            _keyLabel.RegisterCallback<MouseDownEvent>(ShowKeySelectionWindow, TrickleDown.TrickleDown);

            UpdateKeyFieldDisplay(_selectedEntry?.Key);

            DrawTranslationsDictionary(container);

            return container;
        }

        private void ShowKeySelectionWindow(MouseDownEvent _)
        {
            LocalizationSelectorWindow.ShowWindow(_selectedEntry, OnSelectionChanged);
        }

        private void OnSelectionChanged(LocalizationEntry selectedEntry)
        {
            _guidProperty.stringValue = selectedEntry.GUID;

            CopyTranslations(selectedEntry);

            _serializedProperty.serializedObject.ApplyModifiedProperties();

            UpdateKeyFieldDisplay(selectedEntry.Key);

            _selectedEntry = selectedEntry;
        }

        private void DrawTranslationsDictionary(VisualElement container)
        {
            var dictionaryField = new PropertyField(_translationsProperty);
            dictionaryField.BindProperty(_translationsProperty);

            container.Add(dictionaryField);
        }

        private void CopyTranslations(LocalizationEntry entry)
        {
            var serializedList = _translationsProperty.FindPropertyRelative("_serializedList");

            serializedList.ClearArray();

            var index = 0;
            foreach (var translation in entry.Translations)
            {
                serializedList.InsertArrayElementAtIndex(index);
                var element = serializedList.GetArrayElementAtIndex(index);

                var keyProperty = element.FindPropertyRelative("Key");
                var valueProperty = element.FindPropertyRelative("Value");

                keyProperty.intValue = (int)translation.Key;
                valueProperty.stringValue = translation.Value;

                index++;
            }
        }

        private void UpdateKeyFieldDisplay(string key)
        {
            var displayText = string.IsNullOrEmpty(key) ? "[None]" : key;
            _keyLabel.value = displayText;
        }
    }
}