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
    /// Reactive localization controller with SystemLanguage support.
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
            ReadLocalizationData();

            var systemLanguage = Application.systemLanguage;
            if (HasLanguage(systemLanguage))
                Language.Value = systemLanguage;
        }

        /// <summary>
        /// Gets the localized text for the specified key using the current language.
        /// </summary>
        [UsedImplicitly]
        public static string Localize(string localizationKey) => Localize(localizationKey, Language.Value);

        /// <summary>
        /// Gets the localized text for the specified key and language.
        /// </summary>
        [UsedImplicitly]
        public static string Localize(string localizationKey, SystemLanguage language)
        {
            if (string.IsNullOrEmpty(localizationKey))
                return localizationKey;

            if (TryGetTranslation(language, localizationKey, out var translation))
                return translation;

            Debug.LogWarning("[LocalizationController::Localize] " +
                             $"Translation not found: {localizationKey} ({language})");

            return GetFallbackTranslation(localizationKey);
        }

        /// <summary>
        /// Checks if a localization key exists for the current language.
        /// </summary>
        [UsedImplicitly]
        public static bool HasKey(string localizationKey) =>
            _dictionary.ContainsKey(Language.Value) &&
            _dictionary[Language.Value].ContainsKey(localizationKey);

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

            return _keys.AsValueEnumerable().OrderBy(key => key).ToArray();
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
                Debug.LogWarning("[LocalizationController] No localization settings or sheets found");
                return;
            }

            _dictionary.Clear();

            var processedKeys = new HashSet<string>();
            foreach (var sheet in settings.Sheets)
            {
                if (!sheet?.TextAsset)
                {
                    Debug.LogWarning($"[LocalizationController] Sheet '{sheet?.Name}' has no TextAsset");
                    continue;
                }

                LocalizationSheetProcessor.ProcessSheet(sheet, _dictionary, processedKeys);
            }
        }

        private static bool TryGetTranslation(SystemLanguage language, string key, out string translation)
        {
            translation = null;

            if (_dictionary.TryGetValue(language, out var languageDict) is false)
                return false;

            if (languageDict.TryGetValue(key, out translation) is false)
                return false;

            return string.IsNullOrEmpty(translation) is false;
        }

        private static string GetFallbackTranslation(string key)
            => TryGetTranslation(SystemLanguage.English, key, out var fallback) ? fallback : key;
    }
}