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
        private string _selectedSheetForGuidGeneration = string.Empty;

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

            DisplayGuidGenerationSection();

            DisplayCopyAllTextSection();
        }

        private void DisplayGuidGenerationSection()
        {
            EditorGUILayout.Space();
            EditorVisualControls.LabelField("GUID Generation");

            DrawSheetSelection();

            EditorGUILayout.Space();

            if (EditorVisualControls.Button("Generate GUIDs for Selected Sheet"))
                GenerateGuidsForSheet();

            if (EditorVisualControls.Button("Generate GUIDs for All Keys"))
                GenerateGuidsForAllKeys();
        }

        private void DrawSheetSelection()
        {
            var sheets = Database.Sheets;
            if (sheets == null || sheets.Count == 0)
            {
                EditorGUILayout.LabelField("No sheets available. Add sheets first.");
                return;
            }

            var sheetNames = sheets.AsValueEnumerable()
                .Select(static sheet => sheet.Name).ToArray();

            var currentIndex = Array.IndexOf(sheetNames, _selectedSheetForGuidGeneration);
            if (currentIndex == -1 && sheetNames.Length > 0)
                currentIndex = 0;

            var newIndex = EditorGUILayout.Popup("Sheet", currentIndex, sheetNames);
            _selectedSheetForGuidGeneration = sheetNames[newIndex];
        }

        private void GenerateGuidsForSheet()
        {
            if (string.IsNullOrEmpty(_selectedSheetForGuidGeneration))
            {
                EditorUtility.DisplayDialog("Error", "Please select a sheet first.", "OK");
                return;
            }

            var csvContent = LocalizationGuidGenerator.GenerateGuidsForSheet(_selectedSheetForGuidGeneration);

            if (string.IsNullOrEmpty(csvContent))
            {
                EditorUtility.DisplayDialog("Error",
                    $"Failed to generate GUIDs for sheet '{_selectedSheetForGuidGeneration}'.", "OK");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = csvContent;

            EditorUtility.DisplayDialog("Success",
                $"Generated GUIDs for sheet '{_selectedSheetForGuidGeneration}' and copied to clipboard.\n\n" +
                "You can now paste this into your Google Sheet.", "OK");
        }

        private void GenerateGuidsForAllKeys()
        {
            var csvContent = LocalizationGuidGenerator.GenerateGuidsForExistingKeys();

            if (string.IsNullOrEmpty(csvContent))
            {
                EditorUtility.DisplayDialog("Error", "No localization keys found.", "OK");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = csvContent;

            EditorUtility.DisplayDialog("Success",
                "Generated GUIDs for all localization keys and copied to clipboard.\n\n" +
                "This is a simple GUID-Key mapping. For full sheet export, use 'Generate GUIDs for Selected Sheet'.",
                "OK");
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
                .Select(static lang => lang.ToString()).ToArray();

            var currentIndex = Array.IndexOf(languageStrings, _selectedLanguage.ToString());

            if (currentIndex == -1)
                currentIndex = 0;

            var newIndex = EditorGUILayout.Popup("Language", currentIndex, languageStrings);
            _selectedLanguage = availableLanguages[newIndex];
        }

        private void CopyAllTextForLanguage(SystemLanguage language, bool includeKeys)
        {
            var allEntries = LocalizationRegistry.Instance.Entries;
            if (allEntries == null || allEntries.Count == 0)
            {
                EditorUtility.DisplayDialog("Warning", "No localization entries found.", "OK");
                return;
            }

            var textBuilder = new StringBuilder();
            var copiedCount = 0;

            foreach (var entry in allEntries)
            {
                if (entry.TryGetTranslation(language, out var localizedText) is false ||
                    string.IsNullOrEmpty(localizedText))
                    continue;

                var line = includeKeys ? $"{entry.Key}: {localizedText}" : localizedText;
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