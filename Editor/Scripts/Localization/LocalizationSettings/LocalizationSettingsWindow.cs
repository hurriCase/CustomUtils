using System;
using System.Collections.Generic;
using CustomUtils.Editor.Scripts.SheetsDownloader;
using CustomUtils.Runtime.Downloader;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using CustomUtils.Runtime.ResponseTypes;
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

            UpdateChoices(_elements.SheetSelectionDropdown, Database.Sheets, "sheets");

            var availableLanguages = LocalizationController.GetAllLanguages();
            UpdateChoices(_elements.LanguageSelectionDropdown, availableLanguages, "languages");
        }

        protected override void CreateCustomContent()
        {
            _customLayout.CloneTree(CustomContentSlot);
            _elements = new LocalizationSettingsElements(CustomContentSlot);

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
            UpdateChoices(_elements.SheetSelectionDropdown, Database.Sheets, "sheets");

            _elements.ExportSheetButton.clicked += ExportSheet;
            _elements.ExportAllKeysButton.clicked += ExportAllKeys;
        }

        private void SetupCopyAllTextSection()
        {
            var availableLanguages = LocalizationController.GetAllLanguages();
            UpdateChoices(_elements.LanguageSelectionDropdown, availableLanguages, "languages");

            _elements.CopyAllTextButton.clicked += () =>
            {
                var result = CopyAllTextForLanguage();
                result.DisplayMessage();
            };
        }

        private void UpdateChoices<T>(DropdownField dropdownField, List<T> choices, string valueName)
        {
            if (choices.Count == 0)
            {
                var noChoiceMessage = ZString.Format("No {0} available", valueName);
                dropdownField.choices = new List<string> { noChoiceMessage };
                dropdownField.value = noChoiceMessage;
                return;
            }

            var choiceNames = choices.Select(static choice => choice.ToString()).ToList();
            dropdownField.choices = choiceNames;

            if (choiceNames.Count > 0)
                dropdownField.value = choiceNames[0];
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

        private Result CopyAllTextForLanguage()
        {
            var selectedLanguageString = _elements.LanguageSelectionDropdown.value;

            if (string.IsNullOrEmpty(selectedLanguageString) || selectedLanguageString == "No languages available")
                return Result.Invalid("Please select a language first.");

            if (Enum.TryParse<SystemLanguage>(selectedLanguageString, out var language) is false)
                return Result.Invalid("Invalid language selection.");

            var allEntries = LocalizationRegistry.Instance.Entries.Values;

            if (allEntries.Count == 0)
                return Result.Invalid("No localization entries found.");

            using var textBuilder = ZString.CreateStringBuilder();
            var copiedCount = 0;

            foreach (var entry in allEntries)
            {
                if (entry.TryGetTranslation(language, out var localizedText) is false ||
                    string.IsNullOrEmpty(localizedText))
                    continue;

                textBuilder.AppendLine(localizedText);

                copiedCount++;
            }

            if (copiedCount == 0)
                return Result.Invalid($"No translations found for {language}.");

            EditorGUIUtility.systemCopyBuffer = textBuilder.ToString();
            return Result.Valid($"Copied {copiedCount} text entries for {language} to clipboard.");
        }
    }
}