using System;
using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.Downloader;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    internal static class LocalizationSheetProcessor
    {
        private const string GuidColumnName = "GUID";
        private const string KeyColumnName = "Key";

        internal static void ProcessSheet(
            Sheet sheet,
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            HashSet<string> processedKeys)
        {
            var lines = LocalizationCsvParser.ParseLines(sheet.TextAsset.text);
            if (lines.Count == 0)
                return;

            var headerColumns = LocalizationCsvParser.ParseColumns(lines[0]);
            var guidColumnIndex = FindColumnIndex(headerColumns, GuidColumnName);
            var keyColumnIndex = FindColumnIndex(headerColumns, KeyColumnName);

            if (guidColumnIndex == -1 || keyColumnIndex == -1)
            {
                Debug.LogError($"[LocalizationSheetProcessor] Sheet '{sheet.Name}' must have 'GUID' and 'Key' columns");
                return;
            }

            var languages = ParseLanguageHeader(headerColumns, guidColumnIndex, keyColumnIndex);
            if (ValidateLanguages(languages, sheet.Name) is false)
                return;

            InitializeLanguageDictionaries(dictionary, languages);
            ProcessDataRows(lines, headerColumns, guidColumnIndex, keyColumnIndex, languages,
                dictionary, processedKeys, sheet.Name);
        }

        private static int FindColumnIndex(List<string> columns, string columnName)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                if (columns[i].Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        private static List<SystemLanguage> ParseLanguageHeader(
            List<string> headerColumns,
            int guidColumnIndex,
            int keyColumnIndex)
        {
            var systemLanguages = new List<SystemLanguage>();

            for (var i = 0; i < headerColumns.Count; i++)
            {
                if (i == guidColumnIndex || i == keyColumnIndex)
                    continue;

                if (TryParseSystemLanguage(headerColumns[i], out var systemLang))
                {
                    systemLanguages.Add(systemLang);
                    continue;
                }

                Debug.LogWarning($"[LocalizationSheetProcessor::ParseLanguageHeader] " +
                                 $"Unknown language '{headerColumns[i]}' - skipping");
            }

            return systemLanguages;
        }

        private static bool TryParseSystemLanguage(string languageName, out SystemLanguage systemLanguage) =>
            Enum.TryParse(languageName, true, out systemLanguage);

        private static bool ValidateLanguages(List<SystemLanguage> languages, string sheetName)
        {
            if (languages.Count == languages.Distinct().Count())
                return true;

            Debug.LogError("[LocalizationSheetProcessor::ValidateLanguages] " +
                           $"Duplicated languages found in '{sheetName}'. Sheet not loaded.");
            return false;
        }

        private static void InitializeLanguageDictionaries(
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            List<SystemLanguage> languages)
        {
            foreach (var language in languages)
            {
                if (dictionary.ContainsKey(language) is false)
                    dictionary[language] = new Dictionary<string, string>();
            }
        }

        private static void ProcessDataRows(
            List<string> lines,
            List<string> headerColumns,
            int guidColumnIndex,
            int keyColumnIndex,
            List<SystemLanguage> languages,
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            HashSet<string> processedKeys,
            string sheetName)
        {
            for (var i = 1; i < lines.Count; i++)
            {
                var columns = LocalizationCsvParser.ParseColumns(lines[i]);

                if (columns.Count <= guidColumnIndex || columns.Count <= keyColumnIndex)
                    continue;

                var guid = columns[guidColumnIndex];
                var key = columns[keyColumnIndex];

                if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(key))
                    continue;

                if (processedKeys.Add(guid) is false)
                {
                    Debug.LogError("[LocalizationSheetProcessor::ProcessDataRows] " +
                                   $"Duplicated GUID '{guid}' found in '{sheetName}'. Entry not loaded.");
                    continue;
                }

                var entry = new LocalizationEntry(guid, key, sheetName);
                AddTranslationsForEntry(entry, columns, headerColumns, languages, dictionary, sheetName);

                LocalizationRegistry.Instance.AddOrUpdateEntry(entry);
            }
        }

        private static void AddTranslationsForEntry(
            LocalizationEntry entry,
            List<string> columns,
            List<string> headerColumns,
            List<SystemLanguage> languages,
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            string sheetName)
        {
            foreach (var language in languages)
            {
                var languageColumnIndex = FindColumnIndex(headerColumns, language.ToString());
                if (languageColumnIndex == -1 || languageColumnIndex >= columns.Count)
                    continue;

                var translation = columns[languageColumnIndex];

                if (dictionary[language].ContainsKey(entry.Guid))
                {
                    Debug.LogError("[LocalizationSheetProcessor::AddTranslationsForEntry] " +
                                   $"Duplicated GUID '{entry.Guid}' in '{sheetName}' for language '{language}'.");
                    continue;
                }

                dictionary[language][entry.Guid] = translation;
                entry.SetTranslation(language, translation);
            }
        }
    }
}