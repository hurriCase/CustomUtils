using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Reactive localization controller with GUID-based key support.
    /// Manages localized text retrieval and language switching for Unity applications.
    /// </summary>
    [UsedImplicitly]
    public static class LocalizationController
    {
        /// <summary>
        /// Reactive property for current language with automatic localization updates.
        /// </summary>
        [UsedImplicitly]
        public static ReactiveProperty<SystemLanguage> Language { get; } =
            new(LocalizationDatabase.Instance.DefaultLanguage);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInRuntime()
        {
            var systemLanguage = Application.systemLanguage;
            if (HasLanguage(systemLanguage))
                Language.Value = systemLanguage;
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
            if (localizationKey.IsValid is false
                || LocalizationRegistry.Instance.Entries.TryGetValue(localizationKey.GUID, out var entry) is false)
                return string.Empty;

            if (entry.TryGetTranslation(language, out var translation) &&
                string.IsNullOrEmpty(translation) is false)
                return translation;

            if (entry.TryGetTranslation(SystemLanguage.English, out var fallback) &&
                string.IsNullOrEmpty(fallback) is false)
                return fallback;

            return entry.Key;
        }

        /// <summary>
        /// Checks if localization data exists for the specified language.
        /// </summary>
        [UsedImplicitly]
        public static bool HasLanguage(SystemLanguage language)
        {
            foreach (var localizationEntry in LocalizationRegistry.Instance.Entries.Values)
            {
                if (localizationEntry.HasTranslation(language))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves all localization keys across all available languages.
        /// </summary>
        [UsedImplicitly]
        public static string[] GetAllKeys() =>
            LocalizationRegistry.Instance.Entries.Values
                .Select(static localizationEntry => localizationEntry.Key)
                .OrderBy(static guid => guid)
                .ToArray();

        /// <summary>
        /// Retrieves all supported languages available in the localization dictionary.
        /// </summary>
        [UsedImplicitly]
        public static SystemLanguage[] GetAllLanguages()
        {
            var languages = new HashSet<SystemLanguage>();

            foreach (var entry in LocalizationRegistry.Instance.Entries.Values)
            {
                foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
                {
                    if (entry.HasTranslation(language))
                        languages.Add(language);
                }
            }

            return languages.ToArray();
        }

        internal static void ReadLocalizationData()
        {
            var settings = LocalizationDatabase.Instance;

            if (settings.Sheets is null || settings.Sheets.Count == 0)
            {
                Debug.LogWarning("[LocalizationController] No localization sheets found");
                return;
            }

            LocalizationSheetProcessor.ProcessSheets(settings.Sheets);
        }
    }
}