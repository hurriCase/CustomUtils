using System;
using System.Text;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.SheetsDownloader;
using CustomUtils.Runtime.Downloader;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal sealed class LocalizationSettingsWindow : SheetsDownloaderWindowBase<LocalizationDatabase, Sheet>
    {
        private Vector2 _scrollPosition;
        private SystemLanguage _selectedLanguage = SystemLanguage.English;

        protected override LocalizationDatabase Database => LocalizationDatabase.Instance;

        [MenuItem(MenuItemNames.LocalizationMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<LocalizationSettingsWindow>(nameof(LocalizationSettingsWindow).ToSpacedWords());
        }

        protected override void OnSheetsDownloaded()
        {
            LocalizationController.ReadLocalizationData();
        }

        protected override void DrawCustomContent()
        {
            using var scrollScope = EditorVisualControls.CreateScrollView(ref _scrollPosition);

            PropertyField(nameof(LocalizationDatabase.DefaultLanguage));

            DrawCommonSheetsSection();

            DisplayCopyAllTextSection();
        }

        private void DisplayCopyAllTextSection()
        {
            EditorGUILayout.Space();

            DrawCopyAllSection();

            EditorGUILayout.Space();

            if (EditorVisualControls.Button("Copy All Text"))
                CopyAllTextForLanguage(_selectedLanguage, includeKeys: false);

            if (EditorVisualControls.Button("Copy with Keys"))
                CopyAllTextForLanguage(_selectedLanguage, includeKeys: true);
        }

        private void DrawCopyAllSection()
        {
            EditorVisualControls.LabelField("Copy All Text");

            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            var availableLanguages = LocalizationController.GetAllLanguages();
            if (availableLanguages is null || availableLanguages.Length == 0)
            {
                EditorGUILayout.LabelField("No languages available. Download sheets first.");
                return;
            }

            var languageStrings = availableLanguages.AsValueEnumerable()
                .Select(lang => lang.ToString()).ToArray();

            var currentIndex = Array.IndexOf(languageStrings, _selectedLanguage.ToString());

            if (currentIndex == -1)
                currentIndex = 0;

            var newIndex = EditorGUILayout.Popup("Language", currentIndex, languageStrings);
            _selectedLanguage = availableLanguages[newIndex];
        }

        private void CopyAllTextForLanguage(SystemLanguage language, bool includeKeys)
        {
            var allKeys = LocalizationController.GetAllKeys();
            if (allKeys == null || allKeys.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "No localization keys found.", "OK");
                return;
            }

            var textBuilder = new StringBuilder();
            var copiedCount = 0;

            foreach (var key in allKeys)
            {
                var localizedText = LocalizationController.Localize(key, language);

                if (string.IsNullOrEmpty(localizedText) || localizedText == key)
                    continue;

                var line = includeKeys ? $"{key}: {localizedText}" : localizedText;
                textBuilder.AppendLine(line);
                copiedCount++;
            }

            if (copiedCount == 0)
            {
                EditorUtility.DisplayDialog("Warning",
                    $"No translations found for {language}.", "OK");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = textBuilder.ToString();

            var contentType = includeKeys ? "key-value pairs" : "text entries";
            EditorUtility.DisplayDialog("Success",
                $"Copied {copiedCount} {contentType} for {language} to clipboard.", "OK");
        }
    }
}