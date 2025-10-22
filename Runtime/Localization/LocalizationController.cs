using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Reactive localization controller with GUID-based key support.
    /// Manages localized text retrieval and language switching for Unity applications.
    /// </summary>
    [UsedImplicitly]
    public static class LocalizationController
    {
        private static readonly Dictionary<SystemLanguage, Dictionary<string, string>> _dictionary = new();

        /// <summary>
        /// Reactive property for current language with automatic localization updates.
        /// </summary>
        [UsedImplicitly]
        public static ReactiveProperty<SystemLanguage> Language { get; } =
            new(LocalizationDatabase.Instance.DefaultLanguage);

        private static readonly HashSet<string> _keys = new();

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeInEditor() => ReadLocalizationData();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInRuntime()
        {
            BuildDictionaryFromRegistry();

            var systemLanguage = Application.systemLanguage;
            if (HasLanguage(systemLanguage))
                Language.Value = systemLanguage;
        }

        private static void BuildDictionaryFromRegistry()
        {
            _dictionary.Clear();
            var availableLanguages = new HashSet<SystemLanguage>();

            foreach (var entry in LocalizationRegistry.Instance.Entries)
            {
                foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
                {
                    if (entry.HasTranslation(language))
                        availableLanguages.Add(language);
                }
            }

            foreach (var language in availableLanguages)
            {
                if (_dictionary.ContainsKey(language) is false)
                    _dictionary[language] = new Dictionary<string, string>();
            }

            foreach (var entry in LocalizationRegistry.Instance.Entries)
            {
                foreach (var language in availableLanguages)
                {
                    if (entry.TryGetTranslation(language, out var translation))
                        _dictionary[language][entry.Guid] = translation;
                }
            }
        }

        /// <summary>
        /// Gets the localized text for the specified localization key using the current language.
        /// </summary>
        [UsedImplicitly]
        public static string Localize(LocalizationKey localizationKey) =>
            Localize(localizationKey, Language.Value);

        /// <summary>
        /// Gets the localized text for the specified localization key and language.
        /// </summary>
        [UsedImplicitly]
        public static string Localize(LocalizationKey localizationKey, SystemLanguage language)
        {
            if (localizationKey.IsValid is false)
                return localizationKey.Key;

            if (LocalizationRegistry.Instance.TryGetEntry(localizationKey.Guid, out var entry) is false)
            {
                Debug.LogWarning(
                    $"[LocalizationController::Localize] Entry not found for GUID: {localizationKey.Guid}");
                return localizationKey.Key;
            }

            if (entry.TryGetTranslation(language, out var translation) &&
                string.IsNullOrEmpty(translation) is false)
                return translation;

            if (entry.TryGetTranslation(SystemLanguage.English, out var fallback) &&
                string.IsNullOrEmpty(fallback) is false)
                return fallback;

            return localizationKey.Key;
        }

        /// <summary>
        /// Checks if a localization key exists in the registry.
        /// </summary>
        [UsedImplicitly]
        public static bool HasKey(LocalizationKey localizationKey) =>
            localizationKey.IsValid &&
            LocalizationRegistry.Instance.TryGetEntry(localizationKey.Guid, out _);

        /// <summary>
        /// Checks if localization data exists for the specified language.
        /// </summary>
        [UsedImplicitly]
        public static bool HasLanguage(SystemLanguage language) => _dictionary.ContainsKey(language);

        /// <summary>
        /// Retrieves all localization keys across all available languages.
        /// </summary>
        [UsedImplicitly]
        public static string[] GetAllKeys()
        {
            _keys.Clear();
            foreach (var localizations in _dictionary.Values)
            {
                foreach (var key in localizations.Keys)
                    _keys.Add(key);
            }

            return _keys.AsValueEnumerable().OrderBy(static key => key).ToArray();
        }

        /// <summary>
        /// Retrieves all supported languages available in the localization dictionary.
        /// </summary>
        [UsedImplicitly]
        public static SystemLanguage[] GetAllLanguages() => _dictionary.Keys.ToArray();

        internal static void ReadLocalizationData()
        {
            var settings = LocalizationDatabase.Instance;
            if (!settings || settings.Sheets == null)
            {
                Debug.LogWarning("[LocalizationController::ReadLocalizationData] " +
                                 "No localization settings or sheets found");
                return;
            }

            LocalizationRegistry.Instance.Clear();
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

                LocalizationSheetProcessor.ProcessSheet(sheet, _dictionary, processedKeys);
            }

            LocalizationRegistry.Instance.Initialize();

            Debug.Log("[LocalizationController::ReadLocalizationData] " +
                      $"Loaded {LocalizationRegistry.Instance.Entries.Count} " +
                      $"localization entries from {settings.Sheets.Count} sheets");
        }
    }
}