using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Reactive localization controller with automatic font management.
    /// </summary>
    [UsedImplicitly]
    public static class LocalizationController
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _dictionary = new();

        /// <summary>
        /// Reactive property for current language with automatic localization updates.
        /// </summary>
        [UsedImplicitly]
        public static ReactiveProperty<string> Language { get; } = new(LocalizationSettings.Instance.DefaultLanguage);

        /// <summary>
        /// Checks if a localization key exists for the current language.
        /// </summary>
        /// <param name="localizationKey">The key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        [UsedImplicitly]
        public static bool HasKey(string localizationKey) =>
            _dictionary.ContainsKey(Language.Value) &&
            _dictionary[Language.Value].ContainsKey(localizationKey);

        /// <summary>
        /// Checks if a language exists in the localization data.
        /// </summary>
        /// <param name="language">The language to check.</param>
        /// <returns>True if the language exists, false otherwise.</returns>
        [UsedImplicitly]
        public static bool HasLanguage(string language) => _dictionary.ContainsKey(language);

        /// <summary>
        /// Attempts to get font mapping for the specified language.
        /// </summary>
        /// <param name="language">The language to get font mapping for.</param>
        /// <param name="fontMapping">The found font mapping, if any.</param>
        /// <returns>True if font mapping was found, false otherwise.</returns>
        [UsedImplicitly]
        public static bool TryGetFontForLanguage(string language, out LanguageFontMapping fontMapping)
        {
            fontMapping = null;

            var fontMappings = LocalizationSettings.Instance.FontMappings;
            if (fontMappings == null)
                return false;

            fontMapping = fontMappings.AsValueEnumerable()
                .FirstOrDefault(mapping => mapping.Language.Equals(language, StringComparison.OrdinalIgnoreCase));

            return fontMapping != null;
        }

        /// <summary>
        /// Gets localized text for the specified key.
        /// </summary>
        /// <param name="localizationKey">The localization key.</param>
        /// <returns>The localized text or fallback text if not found.</returns>
        [UsedImplicitly]
        public static string Localize(string localizationKey)
        {
            if (string.IsNullOrEmpty(localizationKey))
                return localizationKey;

            if (_dictionary.ContainsKey(Language.Value) is false)
            {
                Debug.LogError($"[LocalizationController::Localize] Language not found: {Language.Value}");
                return localizationKey;
            }

            var languageDict = _dictionary[Language.Value];
            if (languageDict.ContainsKey(localizationKey) &&
                string.IsNullOrEmpty(languageDict[localizationKey]) is false)
                return languageDict[localizationKey];

            Debug.LogWarning("[LocalizationController::Localize] " +
                             $"Translation not found: {localizationKey} ({Language.Value})");

            return GetFallbackText(localizationKey);
        }

        /// <summary>
        /// Gets localized text with string formatting.
        /// </summary>
        /// <param name="localizationKey">The localization key.</param>
        /// <param name="args">Arguments for string formatting.</param>
        /// <returns>The formatted localized text.</returns>
        [UsedImplicitly]
        public static string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            try
            {
                return string.Format(pattern, args);
            }
            catch (FormatException ex)
            {
                Debug.LogError("[LocalizationController::Localize] " +
                               $"Format error for key '{localizationKey}': {ex.Message}");
                return pattern;
            }
        }

        internal static string[] GetAllKeys()
        {
            var allKeys = new HashSet<string>();
            foreach (var languageDict in _dictionary.Values)
            {
                foreach (var key in languageDict.Keys)
                    allKeys.Add(key);
            }

            return allKeys.OrderBy(k => k).ToArray();
        }

        internal static string[] GetAllLanguages() => _dictionary.Keys.OrderBy(language => language).ToArray();

        internal static string GetLocalizedText(string key, string language)
        {
            if (_dictionary.ContainsKey(language) && _dictionary[language].ContainsKey(key))
                return _dictionary[language][key];

            return key;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticMembers()
        {
            _dictionary.Clear();

            Language.Value = "English";
        }

        private static string GetFallbackText(string localizationKey)
        {
            if (_dictionary.ContainsKey("English") &&
                _dictionary["English"].ContainsKey(localizationKey))
                return _dictionary["English"][localizationKey];

            return localizationKey;
        }

        internal static void ReadLocalizationData()
        {
            var settings = LocalizationSettings.Instance;
            if (!settings || settings.Sheets == null)
            {
                Debug.LogWarning("[LocalizationController::ReadLocalizationData] " +
                                 "No localization settings or sheets found");
                return;
            }

            var processedKeys = new HashSet<string>();

            foreach (var sheet in settings.Sheets)
            {
                if (!sheet?.TextAsset)
                {
                    Debug.LogWarning("[LocalizationController::ReadLocalizationData] " +
                                     $"Sheet '{sheet?.Name}' has no TextAsset");
                    continue;
                }

                ProcessSheet(sheet, processedKeys);
            }
        }

        private static void ProcessSheet(Sheet sheet, HashSet<string> processedKeys)
        {
            var lines = ParseCsvLines(sheet.TextAsset.text);
            if (lines.Count == 0)
                return;

            var languages = ParseLanguageHeader(lines[0]);
            if (ValidateLanguages(languages, sheet.Name) is false)
                return;

            InitializeLanguageDictionaries(languages);
            ProcessDataRows(lines, languages, processedKeys, sheet.Name);
        }

        private static List<string> ParseCsvLines(string csvText)
        {
            csvText = csvText.Replace("\r\n", "\n").Replace("\"\"", "[_quote_]");

            var matches = Regex.Matches(csvText, "\"[\\s\\S]+?\"");
            foreach (Match match in matches)
            {
                csvText = csvText.Replace(match.Value,
                    match.Value.Replace("\"", string.Empty)
                        .Replace(",", "[_comma_]")
                        .Replace("\n", "[_newline_]"));
            }

            csvText = ProcessAsianTextSpacing(csvText);

            return csvText.Split('\n')
                .AsValueEnumerable()
                .Where(line => string.IsNullOrEmpty(line) is false)
                .ToList();
        }

        private static string ProcessAsianTextSpacing(string text) =>
            text.Replace("。", "。 ")
                .Replace("、", "、 ")
                .Replace("：", "： ")
                .Replace("！", "！ ")
                .Replace("（", " （")
                .Replace("）", "） ")
                .Trim();

        private static List<string> ParseLanguageHeader(string headerLine) =>
            headerLine.Split(',')
                .AsValueEnumerable()
                .Select(lang => lang.Trim())
                .ToList();

        private static bool ValidateLanguages(List<string> languages, string sheetName)
        {
            if (languages.Count == languages.Distinct().Count())
                return true;

            Debug.LogError("[LocalizationController::ValidateLanguages] " +
                           $"Duplicated languages found in '{sheetName}'. Sheet not loaded.");
            return false;
        }

        private static void InitializeLanguageDictionaries(List<string> languages)
        {
            for (var i = 1; i < languages.Count; i++)
            {
                var language = languages[i];
                if (_dictionary.ContainsKey(language) is false)
                    _dictionary[language] = new Dictionary<string, string>();
            }
        }

        private static void ProcessDataRows(List<string> lines, List<string> languages,
            HashSet<string> processedKeys, string sheetName)
        {
            for (var i = 1; i < lines.Count; i++)
            {
                var columns = ParseCsvColumns(lines[i]);
                var key = columns.FirstOrDefault();

                if (string.IsNullOrEmpty(key))
                    continue;

                if (processedKeys.Add(key) is false)
                {
                    Debug.LogError("[LocalizationController::ProcessDataRows] " +
                                   $"Duplicated key '{key}' found in '{sheetName}'. Key not loaded.");
                    continue;
                }

                AddTranslationsForKey(key, columns, languages, sheetName);
            }
        }

        private static List<string> ParseCsvColumns(string line) =>
            line.Split(',')
                .AsValueEnumerable()
                .Select(column => column.Trim()
                    .Replace("[_quote_]", "\"")
                    .Replace("[_comma_]", ",")
                    .Replace("[_newline_]", "\n"))
                .ToList();

        private static void AddTranslationsForKey(string key, List<string> columns,
            List<string> languages, string sheetName)
        {
            for (var j = 1; j < languages.Count && j < columns.Count; j++)
            {
                var language = languages[j];
                var translation = columns[j];

                if (_dictionary[language].ContainsKey(key))
                    Debug.LogError($"[LocalizationController::AddTranslationsForKey] " +
                                   $"Duplicated key '{key}' in '{sheetName}' for language '{language}'.");
                else
                    _dictionary[language][key] = translation;
            }
        }
    }
}