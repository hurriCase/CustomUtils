using System;
using CustomUtils.Editor.Scripts.SheetsDownloader;
using CustomUtils.Runtime.Downloader;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using Cysharp.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal sealed class LocalizationSettingsWindow : SheetsDownloaderWindowBase<LocalizationDatabase, Sheet>
    {
        [SerializeField] private VisualTreeAsset _customLayout;

        private EnumField _defaultLanguageField;
        private DropdownField _sheetSelectionDropdown;
        private DropdownField _languageSelectionDropdown;

        protected override LocalizationDatabase Database => LocalizationDatabase.Instance;

        [MenuItem(MenuItemNames.LocalizationMenuName)]
        internal static void ShowWindow()
        {
            var window = GetWindow<LocalizationSettingsWindow>(nameof(LocalizationSettingsWindow).ToSpacedWords());
            window.minSize = new Vector2(400, 600);
        }

        protected override void OnSheetsDownloaded()
        {
            LocalizationController.ReadLocalizationData();
            UpdateLanguageChoices();
            UpdateSheetChoices();
        }

        protected override void CreateCustomContent()
        {
            if (_customLayout is null)
            {
                Debug.LogError("[LocalizationSettingsWindow] Custom layout is not assigned!");
                return;
            }

            _customLayout.CloneTree(CustomContentSlot);

            SetupDefaultLanguageField();
            SetupSheetExportSection();
            SetupCopyAllTextSection();
        }

        private void SetupDefaultLanguageField()
        {
            _defaultLanguageField = CustomContentSlot.Q<EnumField>("DefaultLanguage");
            _defaultLanguageField.Init(Database.DefaultLanguage);
            _defaultLanguageField.value = Database.DefaultLanguage;

            _defaultLanguageField.RegisterValueChangedCallback(evt =>
            {
                Database.DefaultLanguage = (SystemLanguage)evt.newValue;
                EditorUtility.SetDirty(Database);
            });
        }

        private void SetupSheetExportSection()
        {
            _sheetSelectionDropdown = CustomContentSlot.Q<DropdownField>("SheetSelection");
            UpdateSheetChoices();

            var exportSheetButton = CustomContentSlot.Q<Button>("ExportSheetButton");
            exportSheetButton.clicked += ExportSheet;

            var exportAllKeysButton = CustomContentSlot.Q<Button>("ExportAllKeysButton");
            exportAllKeysButton.clicked += ExportAllKeys;
        }

        private void SetupCopyAllTextSection()
        {
            _languageSelectionDropdown = CustomContentSlot.Q<DropdownField>("LanguageSelection");
            UpdateLanguageChoices();

            var copyAllTextButton = CustomContentSlot.Q<Button>("CopyAllTextButton");
            copyAllTextButton.clicked += () => CopyAllTextForLanguage(includeKeys: false);

            var copyWithKeysButton = CustomContentSlot.Q<Button>("CopyWithKeysButton");
            copyWithKeysButton.clicked += () => CopyAllTextForLanguage(includeKeys: true);
        }

        private void UpdateSheetChoices()
        {
            if (_sheetSelectionDropdown is null || Database.Sheets is null || Database.Sheets.Count == 0)
            {
                if (_sheetSelectionDropdown != null)
                {
                    _sheetSelectionDropdown.choices = new System.Collections.Generic.List<string>
                        { "No sheets available" };
                    _sheetSelectionDropdown.value = "No sheets available";
                    _sheetSelectionDropdown.SetEnabled(false);
                }

                return;
            }

            var sheetNames = Database.Sheets.Select(sheet => sheet.Name).ToList();
            _sheetSelectionDropdown.choices = sheetNames;

            if (sheetNames.Count > 0)
            {
                _sheetSelectionDropdown.value = sheetNames[0];
                _sheetSelectionDropdown.SetEnabled(true);
            }
        }

        private void UpdateLanguageChoices()
        {
            if (_languageSelectionDropdown is null)
                return;

            var availableLanguages = LocalizationController.GetAllLanguages();

            if (availableLanguages is null || availableLanguages.Length == 0)
            {
                _languageSelectionDropdown.choices = new System.Collections.Generic.List<string>
                    { "No languages available" };
                _languageSelectionDropdown.value = "No languages available";
                _languageSelectionDropdown.SetEnabled(false);
                return;
            }

            var languageStrings = availableLanguages.Select(lang => lang.ToString()).ToList();
            _languageSelectionDropdown.choices = languageStrings;

            if (languageStrings.Count > 0)
            {
                _languageSelectionDropdown.value = languageStrings[0];
                _languageSelectionDropdown.SetEnabled(true);
            }
        }

        private void ExportSheet()
        {
            var selectedSheet = _sheetSelectionDropdown.value;

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
            var selectedLanguageString = _languageSelectionDropdown.value;

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