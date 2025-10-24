using System;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.SheetsDownloader;
using CustomUtils.Runtime.Downloader;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using Cysharp.Text;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal sealed class LocalizationSettingsWindow : SheetsDownloaderWindowBase<LocalizationDatabase, Sheet>
    {
        private Vector2 _scrollPosition;
        private SystemLanguage _selectedLanguage = SystemLanguage.English;
        private string _selectedSheetForExport = string.Empty;

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

            DisplaySheetExportSection();

            DisplayCopyAllTextSection();
        }

        private void DisplaySheetExportSection()
        {
            EditorGUILayout.Space();
            EditorVisualControls.LabelField("Sheet Export");

            DrawSheetSelection();

            EditorGUILayout.Space();

            if (EditorVisualControls.Button("Export Selected Sheet to CSV"))
                ExportSheet();

            if (EditorVisualControls.Button("Export All Keys to CSV"))
                ExportAllKeys();
        }

        private void DrawSheetSelection()
        {
            var sheets = Database.Sheets;
            if (sheets is null || sheets.Count == 0)
            {
                EditorGUILayout.LabelField("No sheets available. Add sheets first.");
                return;
            }

            var sheetNames = sheets.Select(static sheet => sheet.Name).ToArray();

            var currentIndex = Array.IndexOf(sheetNames, _selectedSheetForExport);
            if (currentIndex == -1 && sheetNames.Length > 0)
                currentIndex = 0;

            var newIndex = EditorGUILayout.Popup("Sheet", currentIndex, sheetNames);
            _selectedSheetForExport = sheetNames[newIndex];
        }

        private void ExportSheet()
        {
            if (string.IsNullOrEmpty(_selectedSheetForExport))
            {
                EditorUtility.DisplayDialog("Error", "Please select a sheet first.", "OK");
                return;
            }

            var csvContent = LocalizationSheetExporter.ExportSheet(_selectedSheetForExport);

            if (string.IsNullOrEmpty(csvContent))
            {
                EditorUtility.DisplayDialog("Error",
                    $"Failed to export sheet '{_selectedSheetForExport}'. Make sure the sheet is loaded.", "OK");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = csvContent;

            EditorUtility.DisplayDialog("Success",
                $"Exported sheet '{_selectedSheetForExport}' to CSV and copied to clipboard.\n\n" +
                "You can now paste this into your Google Sheet.", "OK");
        }

        private void ExportAllKeys()
        {
            var csvContent = LocalizationSheetExporter.ExportAllKeysWithGuids();

            if (string.IsNullOrEmpty(csvContent))
            {
                EditorUtility.DisplayDialog("Error", "No localization entries found. Load sheets first.", "OK");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = csvContent;

            EditorUtility.DisplayDialog("Success",
                "Exported all keys with GUIDs to CSV and copied to clipboard.\n\n" +
                "This is a simple GUID-Key mapping. For full sheet export with translations, use 'Export Selected Sheet to CSV'.",
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

            var languageStrings = availableLanguages.Select(static lang => lang.ToString()).ToArray();

            var currentIndex = Array.IndexOf(languageStrings, _selectedLanguage.ToString());

            if (currentIndex == -1)
                currentIndex = 0;

            var newIndex = EditorGUILayout.Popup("Language", currentIndex, languageStrings);
            _selectedLanguage = availableLanguages[newIndex];
        }

        private void CopyAllTextForLanguage(SystemLanguage language, bool includeKeys)
        {
            var allEntries = LocalizationRegistry.Instance.Entries.Values;
            if (allEntries.Count == 0)
            {
                EditorUtility.DisplayDialog("Warning", "No localization entries found.", "OK");
                return;
            }

            using var textBuilder = ZString.CreateStringBuilder();
            var copiedCount = 0;

            foreach (var entry in allEntries)
            {
                if (entry.TryGetTranslation(language, out var localizedText) is false ||
                    string.IsNullOrEmpty(localizedText))
                    continue;

                if (includeKeys)
                {
                    textBuilder.Append(entry.Key);
                    textBuilder.Append(": ");
                }

                textBuilder.AppendLine(localizedText);

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