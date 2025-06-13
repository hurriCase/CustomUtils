﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Localization controller.
    /// </summary>
    [UsedImplicitly]
    public static class LocalizationController
    {
        /// <summary>
        /// Fired when localization changed.
        /// </summary>
        public static event Action OnLocalizationChanged = () => { };

        private static readonly Dictionary<string, Dictionary<string, string>> _dictionary = new();
        private static string _language = "English";

        /// <summary>
        /// Get or set language.
        /// </summary>
        [UsedImplicitly]
        public static string Language
        {
            get => _language;
            set
            {
                _language = value;
                OnLocalizationChanged();
            }
        }

        /// <summary>
        /// Check if a key exists in localization.
        /// </summary>
        [UsedImplicitly]
        public static bool HasKey(string localizationKey) =>
            _dictionary.ContainsKey(Language) && _dictionary[Language].ContainsKey(localizationKey);

        /// <summary>
        ///     Check if a language exists in localization.
        /// </summary>
        [UsedImplicitly]
        public static bool HasLanguage(string language) =>
            _dictionary.ContainsKey(language);

        [UsedImplicitly]
        public static bool TryGetFontForLanguage(string language, out LanguageFontMapping fontMapping)
        {
            fontMapping = null;

            var fontMappings = LocalizationSettings.Instance.FontMappings;
            if (fontMappings == null)
                return false;

            foreach (var mapping in fontMappings)
            {
                if (mapping.Language.Equals(language, StringComparison.OrdinalIgnoreCase) is false)
                    continue;

                fontMapping = mapping;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Get localized value by localization key.
        /// </summary>
        [UsedImplicitly]
        public static string Localize(string localizationKey)
        {
            if (_dictionary.Count == 0) Read();

            if (_dictionary.ContainsKey(Language) is false)
                throw new KeyNotFoundException("Language not found: " + Language);

            var missed = _dictionary[Language].ContainsKey(localizationKey) is false ||
                         string.IsNullOrEmpty(_dictionary[Language][localizationKey]);

            if (missed is false)
                return _dictionary[Language][localizationKey];

            Debug.LogWarning($"Translation not found: {localizationKey} ({Language}).");

            return _dictionary["English"].ContainsKey(localizationKey)
                ? _dictionary["English"][localizationKey]
                : localizationKey;
        }

        /// <summary>
        /// Get localized value by localization key.
        /// </summary>
        [UsedImplicitly]
        public static string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            return string.Format(pattern, args);
        }

        /// <summary>
        /// Read localization spreadsheets.
        /// </summary>
        private static void Read()
        {
            if (_dictionary.Count > 0) return;

            var keys = new List<string>();

            foreach (var sheet in LocalizationSettings.Instance.Sheets)
            {
                var textAsset = sheet.TextAsset;
                var lines = GetLines(textAsset.text);
                var languages = lines[0].Split(',').AsValueEnumerable().Select(i => i.Trim()).ToList();

                if (languages.Count != languages.Distinct().Count())
                {
                    Debug.LogError($"Duplicated languages found in `{sheet.Name}`. This sheet is not loaded.");
                    continue;
                }

                for (var i = 1; i < languages.Count; i++)
                {
                    if (!_dictionary.ContainsKey(languages[i]))
                        _dictionary.Add(languages[i], new Dictionary<string, string>());
                }

                for (var i = 1; i < lines.Count; i++)
                {
                    var columns = GetColumns(lines[i]);
                    var key = columns[0];

                    if (key == "") continue;

                    if (keys.Contains(key))
                    {
                        Debug.LogError($"Duplicated key `{key}` found in `{sheet.Name}`. This key is not loaded.");
                        continue;
                    }

                    keys.Add(key);

                    for (var j = 1; j < languages.Count; j++)
                    {
                        if (_dictionary[languages[j]].ContainsKey(key))
                            Debug.LogError($"Duplicated key `{key}` in `{sheet.Name}`.");
                        else
                            _dictionary[languages[j]].Add(key, columns[j]);
                    }
                }
            }

            AutoLanguage();
        }

        private static List<string> GetLines(string text)
        {
            text = text.Replace("\r\n", "\n").Replace("\"\"", "[_quote_]");

            var matches = Regex.Matches(text, "\"[\\s\\S]+?\"");

            foreach (Match match in matches)
            {
                text = text.Replace(match.Value,
                    match.Value.Replace("\"", null).Replace(",", "[_comma_]").Replace("\n", "[_newline_]"));
            }

            // Making uGUI line breaks to work in asian texts.
            text = text.Replace("。", "。 ").Replace("、", "、 ").Replace("：", "： ").Replace("！", "！ ").Replace("（", " （")
                .Replace("）", "） ").Trim();

            return text.Split('\n').Where(i => i != "").ToList();
        }

        private static List<string> GetColumns(string line)
        {
            return line.Split(',').Select(j => j.Trim()).Select(j =>
                j.Replace("[_quote_]", "\"").Replace("[_comma_]", ",").Replace("[_newline_]", "\n")).ToList();
        }

        private static void AutoLanguage() => Language = "English";
    }
}