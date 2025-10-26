using System;
using System.Collections.Generic;
using CustomUtils.Editor.Scripts.SheetsDownloader;
using CustomUtils.Runtime.Downloader;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using Cysharp.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization.LocalizationSettings
{
    internal sealed class LocalizationSettingsWindow : SheetsDownloaderWindowBase<LocalizationDatabase, Sheet>
    {
        [SerializeField] private VisualTreeAsset _customLayout;

        private LocalizationSettingsElements _elements;

        protected override LocalizationDatabase Database => LocalizationDatabase.Instance;

        [MenuItem(MenuItemNames.LocalizationMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<LocalizationSettingsWindow>(nameof(LocalizationSettingsWindow).ToSpacedWords());
        }

        protected override void OnSheetsDownloaded()
        {
            LocalizationController.ReadLocalizationData();

            UpdateLanguageChoices();
            UpdateSheetChoices();
        }

        protected override void CreateCustomContent()
        {
            _elements = new LocalizationSettingsElements(CustomContentSlot);

            _customLayout.CloneTree(CustomContentSlot);

            SetupDefaultLanguageField();
            SetupSheetExportSection();
            SetupCopyAllTextSection();
        }

        private void SetupDefaultLanguageField()
        {
            _elements.DefaultLanguageField.Init(Database.DefaultLanguage);
            _elements.DefaultLanguageField.value = Database.DefaultLanguage;

            _elements.DefaultLanguageField.RegisterValueChangedCallback(evt =>
            {
                Database.DefaultLanguage = (SystemLanguage)evt.newValue;
                EditorUtility.SetDirty(Database);
            });
        }

        private void SetupSheetExportSection()
        {
            UpdateSheetChoices();

            _elements.ExportSheetButton.clicked += ExportSheet;
            _elements.ExportAllKeysButton.clicked += ExportAllKeys;
        }

        private void SetupCopyAllTextSection()
        {
            UpdateLanguageChoices();

            _elements.CopyAllTextButton.clicked += () => CopyAllTextForLanguage(includeKeys: false);
            _elements.CopyWithKeysButton.clicked += () => CopyAllTextForLanguage(includeKeys: true);
        }

        private void UpdateSheetChoices()
        {
            if (Database.Sheets is null || Database.Sheets.Count == 0)
            {
                _elements.SheetSelectionDropdown.choices = new List<string> { "No sheets available" };
                _elements.SheetSelectionDropdown.value = "No sheets available";
                _elements.SheetSelectionDropdown.SetEnabled(false);

                return;
            }

            var sheetNames = Database.Sheets.Select(static sheet => sheet.Name).ToList();
            _elements.SheetSelectionDropdown.choices = sheetNames;

            if (sheetNames.Count <= 0)
                return;

            _elements.SheetSelectionDropdown.value = sheetNames[0];
            _elements.SheetSelectionDropdown.SetEnabled(true);
        }

        private void UpdateLanguageChoices()
        {
            var availableLanguages = LocalizationController.GetAllLanguages();

            if (availableLanguages is null || availableLanguages.Length == 0)
            {
                _elements.LanguageSelectionDropdown.choices = new List<string> { "No languages available" };
                _elements.LanguageSelectionDropdown.value = "No languages available";
                _elements.LanguageSelectionDropdown.SetEnabled(false);
                return;
            }

            var languageStrings = availableLanguages.Select(lang => lang.ToString()).ToList();
            _elements.LanguageSelectionDropdown.choices = languageStrings;

            if (languageStrings.Count <= 0)
                return;

            _elements.LanguageSelectionDropdown.value = languageStrings[0];
            _elements.LanguageSelectionDropdown.SetEnabled(true);
        }

        private void ExportSheet()
        {
            var selectedSheet = _elements.SheetSelectionDropdown.value;

            if (string.IsNullOrEmpty(selectedSheet) || selectedSheet == "No sheets available")
            {
                EditorUtility.DisplayDialog("Error", "Please select a sheet first.", "OK");
                return;
            }

            var csvContent = LocalizationSheetExporter.ExportSheet(selectedSheet);

            if (string.IsNullOrEmpty(csvContent))
            {
                EditorUtility.DisplayDialog("Error",
                    $"Failed to export sheet '{selectedSheet}'. Make sure the sheet is loaded.", "OK");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = csvContent;

            EditorUtility.DisplayDialog("Success",
                $"Exported sheet '{selectedSheet}' to CSV and copied to clipboard.\n\n" +
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

        private void CopyAllTextForLanguage(bool includeKeys)
        {
            var selectedLanguageString = _elements.LanguageSelectionDropdown.value;

            if (string.IsNullOrEmpty(selectedLanguageString) || selectedLanguageString == "No languages available")
            {
                EditorUtility.DisplayDialog("Error", "Please select a language first.", "OK");
                return;
            }

            if (Enum.TryParse<SystemLanguage>(selectedLanguageString, out var language) is false)
            {
                EditorUtility.DisplayDialog("Error", "Invalid language selection.", "OK");
                return;
            }

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