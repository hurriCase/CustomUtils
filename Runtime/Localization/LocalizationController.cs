using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CustomUtils.Runtime.Downloader;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using ZLinq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Reactive localization controller with SystemLanguage support.
    /// Manages localized text retrieval and language switching for Unity applications.
    /// </summary>
    /// <remarks>
    /// This static controller automatically initializes localization data from CSV sheets and provides
    /// reactive language switching capabilities. It supports fallback to English when translations
    /// are missing and includes robust CSV parsing for various text formats including Asian languages.
    /// </remarks>
    [UsedImplicitly]
    public static class LocalizationController
    {
        /// <summary>
        /// Internal dictionary storing all localization data organized by language and key.
        /// </summary>
        private static readonly Dictionary<SystemLanguage, Dictionary<string, string>> _dictionary = new();

        /// <summary>
        /// Reactive property for current language with automatic localization updates.
        /// </summary>
        /// <remarks>
        /// Subscribe to this property to receive notifications when the language changes.
        /// All UI elements using localization should react to changes in this property.
        /// </remarks>
        [UsedImplicitly]
        public static ReactiveProperty<SystemLanguage> Language { get; } =
            new(LocalizationDatabase.Instance.DefaultLanguage);

#if UNITY_EDITOR
        /// <summary>
        /// Initializes localization data when Unity Editor loads.
        /// </summary>
        /// <remarks>
        /// This method is called automatically by Unity Editor during domain reload.
        /// Ensures localization data is available during edit-time for inspector previews.
        /// </remarks>
        [InitializeOnLoadMethod]
        private static void InitializeInEditor()
        {
            ReadLocalizationData();
        }
#endif

        /// <summary>
        /// Initializes localization data at runtime before scene loading.
        /// </summary>
        /// <remarks>
        /// This method is called automatically by Unity during application startup.
        /// Sets the current language to the system language if available, otherwise uses default.
        /// </remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInRuntime()
        {
            ReadLocalizationData();

            var systemLanguage = Application.systemLanguage;
            if (HasLanguage(systemLanguage))
                Language.Value = systemLanguage;
        }

        /// <summary>
        /// Attempts to parse a language name string into a SystemLanguage enum value.
        /// </summary>
        /// <param name="languageName">The language name to parse (case-insensitive).</param>
        /// <param name="systemLanguage">When successful, contains the parsed SystemLanguage value.</param>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        private static bool TryParseSystemLanguage(string languageName, out SystemLanguage systemLanguage)
            => Enum.TryParse(languageName, true, out systemLanguage);

        /// <summary>
        /// Checks if a localization key exists for the current language.
        /// </summary>
        /// <param name="localizationKey">The key to check for existence.</param>
        /// <returns>True if the key exists for the current language; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool HasKey(string localizationKey) =>
            _dictionary.ContainsKey(Language.Value) &&
            _dictionary[Language.Value].ContainsKey(localizationKey);

        /// <summary>
        /// Checks if localization data exists for the specified language.
        /// </summary>
        /// <param name="language">The language to check for availability.</param>
        /// <returns>True if the language is supported; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool HasLanguage(SystemLanguage language) => _dictionary.ContainsKey(language);

        /// <summary>
        /// Gets the localized text for the specified key using the current language.
        /// </summary>
        /// <param name="localizationKey">The localization key to retrieve.</param>
        /// <returns>
        /// The localized text if found; otherwise, returns the key itself as fallback.
        /// If the current language doesn't have the key, attempts to use English fallback.
        /// </returns>
        [UsedImplicitly]
        public static string Localize(string localizationKey) => Localize(localizationKey, Language.Value);

        /// <summary>
        /// Gets the localized text for the specified key using the current language with string formatting.
        /// </summary>
        /// <param name="localizationKey">The localization key to retrieve.</param>
        /// <param name="args">Arguments for string.Format to replace placeholders in the localized text.</param>
        /// <returns>
        /// The formatted localized text if found; otherwise, returns the unformatted pattern.
        /// Logs an error if formatting fails and returns the unformatted pattern.
        /// </returns>
        [UsedImplicitly]
        public static string Localize(string localizationKey, params object[] args)
            => Localize(localizationKey, Language.Value, args);

        /// <summary>
        /// Gets the localized text for the specified key and language.
        /// </summary>
        /// <param name="localizationKey">The localization key to retrieve.</param>
        /// <param name="language">The specific language to use for localization.</param>
        /// <returns>
        /// The localized text if found; otherwise, attempts English fallback before returning the key itself.
        /// Logs warnings when translations or languages are not found.
        /// </returns>
        [UsedImplicitly]
        public static string Localize(string localizationKey, SystemLanguage language)
        {
            if (string.IsNullOrEmpty(localizationKey))
                return localizationKey;

            if (_dictionary.TryGetValue(language, out var languageDict) is false)
            {
                Debug.LogError($"[LocalizationController::Localize] Language not found: {language}");
                return localizationKey;
            }

            if (languageDict.ContainsKey(localizationKey) &&
                string.IsNullOrEmpty(languageDict[localizationKey]) is false)
                return languageDict[localizationKey];

            Debug.LogWarning("[LocalizationController::Localize] " +
                             $"Translation not found: {localizationKey} ({language})");

            return GetFallbackText(localizationKey);
        }

        /// <summary>
        /// Gets the localized text for the specified key and language with string formatting.
        /// </summary>
        /// <param name="localizationKey">The localization key to retrieve.</param>
        /// <param name="language">The specific language to use for localization.</param>
        /// <param name="args">Arguments for string.Format to replace placeholders in the localized text.</param>
        /// <returns>
        /// The formatted localized text if found and formatting succeeds; otherwise, returns the unformatted pattern.
        /// Logs an error if formatting fails.
        /// </returns>
        [UsedImplicitly]
        public static string Localize(string localizationKey, SystemLanguage language, params object[] args)
        {
            var pattern = Localize(localizationKey, language);

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

        internal static SystemLanguage[] GetAllLanguages() =>
            _dictionary.Keys.OrderBy(language => language.ToString()).ToArray();

        internal static string GetLocalizedText(string key, SystemLanguage language)
        {
            if (_dictionary.ContainsKey(language) && _dictionary[language].ContainsKey(key))
                return _dictionary[language][key];

            return key;
        }

        internal static void ReadLocalizationData()
        {
            var settings = LocalizationDatabase.Instance;
            if (!settings || settings.Sheets == null)
            {
                Debug.LogWarning("[LocalizationController::ReadLocalizationData] " +
                                 "No localization settings or sheets found");
                return;
            }

            _dictionary.Clear();

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

        /// <summary>
        /// Attempts to get fallback text in English for a localization key.
        /// </summary>
        /// <param name="localizationKey">The key to find fallback text for.</param>
        /// <returns>
        /// The English translation if available; otherwise, returns the key itself.
        /// </returns>
        /// <remarks>
        /// This method is used when a translation is missing in the requested language.
        /// English is used as the universal fallback language.
        /// </remarks>
        private static string GetFallbackText(string localizationKey)
        {
            if (_dictionary.ContainsKey(SystemLanguage.English) &&
                _dictionary[SystemLanguage.English].ContainsKey(localizationKey))
                return _dictionary[SystemLanguage.English][localizationKey];

            return localizationKey;
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
                    systemLanguages.Add(systemLang);
                else
                    Debug.LogWarning("[LocalizationController::ParseLanguageHeader] " +
                                     $"Unknown language '{languageStrings[i]}' - skipping");
            }

            return systemLanguages;
        }

        private static bool ValidateLanguages(List<SystemLanguage> languages, string sheetName)
        {
            if (languages.Count == languages.Distinct().Count())
                return true;

            Debug.LogError("[LocalizationController::ValidateLanguages] " +
                           $"Duplicated languages found in '{sheetName}'. Sheet not loaded.");
            return false;
        }

        private static void InitializeLanguageDictionaries(List<SystemLanguage> languages)
        {
            foreach (var language in languages)
            {
                if (_dictionary.ContainsKey(language) is false)
                    _dictionary[language] = new Dictionary<string, string>();
            }
        }

        private static void ProcessDataRows(List<string> lines, List<SystemLanguage> languages,
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
            List<SystemLanguage> languages, string sheetName)
        {
            for (var j = 0; j < languages.Count && j + 1 < columns.Count; j++)
            {
                var language = languages[j];
                var translation = columns[j + 1]; // +1 because first column is the key

                if (_dictionary[language].ContainsKey(key))
                    Debug.LogError("[LocalizationController::AddTranslationsForKey] " +
                                   $"Duplicated key '{key}' in '{sheetName}' for language '{language}'.");
                else
                    _dictionary[language][key] = translation;
            }
        }
    }
}