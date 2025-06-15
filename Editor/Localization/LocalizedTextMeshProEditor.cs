using System;
using System.Collections.Generic;
using UnityEditor;
using CustomUtils.Runtime.Localization;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
using TMPro;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Localization
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalizedTextMeshPro))]
    internal sealed class LocalizedTextMeshProEditor : EditorBase
    {
        private TextMeshProUGUI _textComponent;
        private string _selectedLanguage;

        private SerializedProperty _localizationKeyProperty;

        private const int MaxShownKeys = 8;

        protected override void InitializeEditor()
        {
            if (target && target is LocalizedTextMeshPro localizedText)
                _textComponent = localizedText.Text;

            _localizationKeyProperty = serializedObject.FindField(nameof(LocalizedTextMeshPro.LocalizationKey));

            _selectedLanguage = LocalizationSettings.Instance.DefaultLanguage;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawKeySelectionWithSearch();

            DrawLanguageSelection();

            if (string.IsNullOrEmpty(_localizationKeyProperty.stringValue) is false &&
                LocalizationController.HasKey(_localizationKeyProperty.stringValue))
                DrawLocalizedContent();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawKeySelectionWithSearch()
        {
            var currentKey = _localizationKeyProperty.stringValue;
            var hasValidKey = LocalizationController.HasKey(currentKey);

            DrawLocalizationKeySection(hasValidKey);

            if (hasValidKey is false && string.IsNullOrEmpty(currentKey) is false)
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
                if (GUILayout.Button($"{key} \u25b2", GUI.skin.label))
                    SelectKey(key);
            }

            if (matchingKeys.Count == MaxShownKeys && totalKeys > MaxShownKeys)
                GUILayout.Label("...and more");
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

            _selectedLanguage =
                EditorStateControls.Dropdown("Preview Language", _selectedLanguage, availableLanguages);

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

        private void DrawLanguageRow(string key, string language)
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            EditorVisualControls.LabelField(language, GUILayout.Width(100));

            var localizedText = LocalizationController.GetLocalizedText(key, language);

            if (EditorVisualControls.Button(localizedText, GUI.skin.textField))
                ApplyPreviewText(localizedText, language);
        }

        private void ApplyPreviewText(string text, string language)
        {
            if (string.IsNullOrEmpty(text) || text == "[Empty]")
                return;

            Undo.RecordObject(_textComponent, "Apply Localized Text Preview");
            _textComponent.text = text.Replace("...", string.Empty);
            EditorUtility.SetDirty(_textComponent);

            ApplyLocalizedFont(language);
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

            ApplyLocalizedFont(_selectedLanguage);
        }

        private void ApplyLocalizedFont(string language)
        {
            if (LocalizationController.TryGetFontForLanguage(language, out var fontMapping) is false ||
                !fontMapping.Font)
                return;

            Undo.RecordObject(_textComponent, "Apply Localized Font");
            _textComponent.font = fontMapping.Font;
            EditorUtility.SetDirty(_textComponent);
        }
    }
}