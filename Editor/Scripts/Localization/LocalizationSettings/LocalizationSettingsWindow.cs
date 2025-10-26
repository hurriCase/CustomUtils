﻿using System;
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

        protected override LocalizationDatabase Database => LocalizationDatabase.Instance;
        private LocalizationRegistry Registry => LocalizationRegistry.Instance;

        private LocalizationSettingsElements _elements;

        [MenuItem(MenuItemNames.LocalizationMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<LocalizationSettingsWindow>(nameof(LocalizationSettingsWindow).ToSpacedWords());
        }

        protected override void OnSheetsDownloaded()
        {
            LocalizationController.ReadLocalizationData();

            UpdateChoices(_elements.SheetSelectionDropdown, Database.Sheets, LocalizationConstants.SheetsValueName);
            UpdateChoices(_elements.LanguageSelectionDropdown, Registry.SupportedLanguages,
                LocalizationConstants.LanguagesValueName);
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
            UpdateChoices(_elements.SheetSelectionDropdown, Database.Sheets,
                LocalizationConstants.SheetsValueName);

            _elements.ExportSheetButton.clicked += () =>
            {
                var result = ExportSheet();
                result.DisplayMessage();
            };
        }

        private void SetupCopyAllTextSection()
        {
            UpdateChoices(_elements.LanguageSelectionDropdown, Registry.SupportedLanguages,
                LocalizationConstants.LanguagesValueName);

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
                var noChoiceMessage = ZString.Format(LocalizationConstants.NoChoiceMessageFormat, valueName);
                dropdownField.choices = new List<string> { noChoiceMessage };
                dropdownField.value = noChoiceMessage;
                return;
            }

            var choiceNames = choices.Select(static choice => choice.ToString()).ToList();
            dropdownField.choices = choiceNames;

            if (choiceNames.Count > 0)
                dropdownField.value = choiceNames[0];
        }

        private Result ExportSheet()
        {
            var selectedSheet = _elements.SheetSelectionDropdown.value;

            if (string.IsNullOrEmpty(selectedSheet) || selectedSheet == LocalizationConstants.NoSheetsAvailable)
                return Result.Invalid(LocalizationConstants.SelectSheetFirstMessage);

            var csvContent = LocalizationSheetExporter.ExportSheet(selectedSheet);

            if (string.IsNullOrEmpty(csvContent))
            {
                var errorMessage = ZString.Format(LocalizationConstants.ExportFailedMessageFormat, selectedSheet);
                return Result.Invalid(errorMessage);
            }

            EditorGUIUtility.systemCopyBuffer = csvContent;
            var successMessage = ZString.Format(LocalizationConstants.ExportSuccessMessageFormat, selectedSheet);
            return Result.Valid(successMessage);
        }

        private Result CopyAllTextForLanguage()
        {
            var selectedLanguageString = _elements.LanguageSelectionDropdown.value;

            if (string.IsNullOrEmpty(selectedLanguageString) ||
                selectedLanguageString == LocalizationConstants.NoLanguagesAvailable)
                return Result.Invalid(LocalizationConstants.SelectLanguageFirstMessage);

            if (Enum.TryParse<SystemLanguage>(selectedLanguageString, out var language) is false)
                return Result.Invalid(LocalizationConstants.InvalidLanguageSelectionMessage);

            var allEntries = Registry.Entries.Values;

            if (allEntries.Count == 0)
                return Result.Invalid(LocalizationConstants.NoLocalizationEntriesMessage);

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
                return Result.Invalid(ZString.Format(LocalizationConstants.NoTranslationsFoundFormat,
                    language));

            EditorGUIUtility.systemCopyBuffer = textBuilder.ToString();
            return Result.Valid(ZString.Format(LocalizationConstants.CopySuccessMessageFormat,
                copiedCount, language));
        }
    }
}