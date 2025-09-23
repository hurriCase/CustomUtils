using System;
using System.Collections.Generic;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using TMPro;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalizedTextMeshPro))]
    internal sealed class LocalizedTextMeshProEditor : EditorBase
    {
        private TextMeshProUGUI _textComponent;
        private SystemLanguage _selectedLanguage;

        private SerializedProperty _localizationKeyProperty;
        private SerializedProperty _textProperty;

        private const int MaxShownKeys = 8;

        protected override void InitializeEditor()
        {
            if (serializedObject.TryGetComponent(out _textComponent) is false)
                return;

            _localizationKeyProperty = serializedObject.FindField(nameof(LocalizedTextMeshPro.LocalizationKey));
            _textProperty = serializedObject.FindField(nameof(LocalizedTextMeshPro.Text));

            _selectedLanguage = LocalizationDatabase.Instance.DefaultLanguage;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawKeySelectionWithSearch();

            DrawLanguageSelection();

            if (LocalizationController.HasKey(_localizationKeyProperty.stringValue))
                DrawLocalizedContent();

            EditorStateControls.PropertyField(_textProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawKeySelectionWithSearch()
        {
            var currentKey = _localizationKeyProperty.stringValue;
            var hasValidKey = LocalizationController.HasKey(currentKey);

            DrawLocalizationKeySection(hasValidKey);

            if (hasValidKey is false && currentKey.IsValid())
                DrawKeySearchSuggestions(currentKey);
        }

        private void DrawLocalizationKeySection(bool hasValidKey)
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            EditorStateControls.PropertyField(_localizationKeyProperty);

            DrawValidationIcon(hasValidKey);
        }

        private void DrawValidationIcon(bool hasValidKey)
        {
            var icon = hasValidKey ? "\u2714" : "\u2718";
            var color = hasValidKey ? Color.green : Color.red;

            GUI.color = color;
            GUILayout.Label(icon, GUI.skin.button, GUILayout.Height(20), GUILayout.Width(22));
            GUI.color = Color.white;
        }

        private void DrawKeySearchSuggestions(string searchText)
        {
            var availableKeys = LocalizationController.GetAllKeys();
            if (availableKeys == null || availableKeys.Length == 0)
                return;

            var matchingKeys = availableKeys.AsValueEnumerable()
                .Where(key => MatchesSearchCriteria(key, searchText))
                .Take(MaxShownKeys)
                .ToArray();

            if (matchingKeys.Length == 0)
                return;

            DrawMatchingKeys(matchingKeys, availableKeys.Length);
        }

        private static bool MatchesSearchCriteria(string key, string searchText) =>
            key.StartsWith(searchText, StringComparison.OrdinalIgnoreCase) ||
            key.Contains(searchText, StringComparison.OrdinalIgnoreCase);

        private void DrawMatchingKeys(IReadOnlyCollection<string> matchingKeys, int totalKeys)
        {
            foreach (var key in matchingKeys)
            {
                if (EditorVisualControls.Button($"{key} \u25b2", GUI.skin.label))
                    SelectKey(key);
            }

            if (matchingKeys.Count == MaxShownKeys && totalKeys > MaxShownKeys)
                EditorVisualControls.LabelField("...and more");
        }

        private void SelectKey(string key)
        {
            _localizationKeyProperty.stringValue = key;
            GUIUtility.keyboardControl = 0;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            ApplyLocalizedText();
        }

        private void DrawLanguageSelection()
        {
            EditorGUI.BeginChangeCheck();

            var availableLanguages = LocalizationController.GetAllLanguages();
            if (availableLanguages == null || availableLanguages.Length == 0)
            {
                EditorVisualControls.WarningBox("No languages found in localization data.");
                return;
            }

            var languageStrings = availableLanguages.AsValueEnumerable()
                .Select(lang => lang.ToString()).ToArray();

            var currentIndex = Array.IndexOf(languageStrings, _selectedLanguage.ToString());

            if (currentIndex == -1) currentIndex = 0;

            var newIndex = EditorStateControls.Dropdown("Preview Language", currentIndex, languageStrings);
            _selectedLanguage = availableLanguages[newIndex];

            if (EditorGUI.EndChangeCheck())
                ApplyLocalizedText();
        }

        private void DrawLocalizedContent()
        {
            var currentKey = _localizationKeyProperty.stringValue;
            var availableLanguages = LocalizationController.GetAllLanguages();

            if (availableLanguages == null || availableLanguages.Length == 0)
                return;

            foreach (var language in availableLanguages)
                DrawLanguageRow(currentKey, language);
        }

        private void DrawLanguageRow(string key, SystemLanguage language)
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            EditorVisualControls.LabelField(language.ToString(), GUILayout.Width(100));

            var localizedText = LocalizationController.GetLocalizedText(key, language);

            if (EditorVisualControls.Button(localizedText, GUI.skin.textField))
                ApplyPreviewText(localizedText);
        }

        private void ApplyPreviewText(string text)
        {
            if (string.IsNullOrEmpty(text) || text == "[Empty]")
                return;

            Undo.RecordObject(_textComponent, "Apply Localized Text Preview");
            _textComponent.text = text.Replace("...", string.Empty);
            EditorUtility.SetDirty(_textComponent);
        }

        private void ApplyLocalizedText()
        {
            if (string.IsNullOrEmpty(_localizationKeyProperty.stringValue))
                return;

            var localizedText =
                LocalizationController.GetLocalizedText(_localizationKeyProperty.stringValue, _selectedLanguage);

            Undo.RecordObject(_textComponent, "Apply Localized Text");
            _textComponent.text = localizedText;
            EditorUtility.SetDirty(_textComponent);
        }
    }
}