using System;
using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.Downloader;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    internal static class LocalizationSheetProcessor
    {
        internal static void ProcessSheet(
            Sheet sheet,
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            HashSet<string> processedKeys)
        {
            var lines = LocalizationCsvParser.ParseLines(sheet.TextAsset.text);
            if (lines.Count == 0)
                return;

            var languages = ParseLanguageHeader(lines[0]);
            if (ValidateLanguages(languages, sheet.Name) is false)
                return;

            InitializeLanguageDictionaries(dictionary, languages);
            ProcessDataRows(lines, languages, dictionary, processedKeys, sheet.Name);
        }

        private static List<SystemLanguage> ParseLanguageHeader(string headerLine)
        {
            var languageStrings = headerLine.Split(',')
                .AsValueEnumerable()
                .Select(lang => lang.Trim())
                .ToList();

            var systemLanguages = new List<SystemLanguage>();

            for (var i = 1; i < languageStrings.Count; i++)
            {
                if (TryParseSystemLanguage(languageStrings[i], out var systemLang))
                {
                    systemLanguages.Add(systemLang);
                    continue;
                }

                Debug.LogWarning("[LocalizationSheetProcessor::ParseLanguageHeader]" +
                                 $" Unknown language '{languageStrings[i]}' - skipping");
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
            List<SystemLanguage> languages,
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            HashSet<string> processedKeys,
            string sheetName)
        {
            for (var i = 1; i < lines.Count; i++)
            {
                var columns = LocalizationCsvParser.ParseColumns(lines[i]);
                var key = columns.FirstOrDefault();

                if (string.IsNullOrEmpty(key))
                    continue;

                if (processedKeys.Add(key) is false)
                {
                    Debug.LogError("[LocalizationSheetProcessor::ProcessDataRows] " +
                                   $"Duplicated key '{key}' found in '{sheetName}'. Key not loaded.");
                    continue;
                }

                AddTranslationsForKey(key, columns, languages, dictionary, sheetName);
            }
        }

        private static void AddTranslationsForKey(
            string key,
            List<string> columns,
            List<SystemLanguage> languages,
            Dictionary<SystemLanguage, Dictionary<string, string>> dictionary,
            string sheetName)
        {
            for (var j = 0; j < languages.Count && j + 1 < columns.Count; j++)
            {
                var language = languages[j];
                var translation = columns[j + 1];

                if (dictionary[language].ContainsKey(key))
                {
                    Debug.LogError("[LocalizationSheetProcessor::AddTranslationsForKey] " +
                                   $"Duplicated key '{key}' in '{sheetName}' for language '{language}'.");
                    continue;
                }

                dictionary[language][key] = translation;
            }
        }
    }
}