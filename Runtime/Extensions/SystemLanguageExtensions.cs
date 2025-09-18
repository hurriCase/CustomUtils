using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    internal static class SystemLanguageExtensions
    {
        private const string IsoEnglish = "en";
        private const string IsoJapanese = "ja";
        private const string IsoKorean = "ko";
        private const string IsoChinese = "zh-CN";
        private const string IsoChineseSimplified = "zh-hans";
        private const string IsoChineseTraditional = "zh-hant";
        private const string IsoSpanish = "es";
        private const string IsoFrench = "fr";
        private const string IsoGerman = "de";
        private const string IsoItalian = "it";
        private const string IsoPortuguese = "pt";
        private const string IsoRussian = "ru";
        private const string IsoArabic = "ar";
        private const string IsoHindi = "hi";
        private const string IsoThai = "th";
        private const string IsoVietnamese = "vi";
        private const string IsoTurkish = "tr";
        private const string IsoPolish = "pl";
        private const string IsoDutch = "nl";
        private const string IsoSwedish = "sv";
        private const string IsoNorwegian = "nb";
        private const string IsoDanish = "da";
        private const string IsoFinnish = "fi";
        private const string IsoCzech = "cs";
        private const string IsoHungarian = "hu";
        private const string IsoGreek = "el";
        private const string IsoHebrew = "he";
        private const string IsoBulgarian = "bg";
        private const string IsoRomanian = "ro";
        private const string IsoSlovak = "sk";
        private const string IsoSlovenian = "sl";
        private const string IsoUkrainian = "uk";
        private const string IsoLithuanian = "lt";
        private const string IsoLatvian = "lv";
        private const string IsoEstonian = "et";

        private static readonly Dictionary<SystemLanguage, string> _systemLanguageToISOMap = new()
        {
            [SystemLanguage.English] = IsoEnglish,
            [SystemLanguage.Japanese] = IsoJapanese,
            [SystemLanguage.Korean] = IsoKorean,
            [SystemLanguage.Chinese] = IsoChinese,
            [SystemLanguage.ChineseSimplified] = IsoChineseSimplified,
            [SystemLanguage.ChineseTraditional] = IsoChineseTraditional,
            [SystemLanguage.Spanish] = IsoSpanish,
            [SystemLanguage.French] = IsoFrench,
            [SystemLanguage.German] = IsoGerman,
            [SystemLanguage.Italian] = IsoItalian,
            [SystemLanguage.Portuguese] = IsoPortuguese,
            [SystemLanguage.Russian] = IsoRussian,
            [SystemLanguage.Arabic] = IsoArabic,
            [SystemLanguage.Hindi] = IsoHindi,
            [SystemLanguage.Thai] = IsoThai,
            [SystemLanguage.Vietnamese] = IsoVietnamese,
            [SystemLanguage.Turkish] = IsoTurkish,
            [SystemLanguage.Polish] = IsoPolish,
            [SystemLanguage.Dutch] = IsoDutch,
            [SystemLanguage.Swedish] = IsoSwedish,
            [SystemLanguage.Norwegian] = IsoNorwegian,
            [SystemLanguage.Danish] = IsoDanish,
            [SystemLanguage.Finnish] = IsoFinnish,
            [SystemLanguage.Czech] = IsoCzech,
            [SystemLanguage.Hungarian] = IsoHungarian,
            [SystemLanguage.Greek] = IsoGreek,
            [SystemLanguage.Hebrew] = IsoHebrew,
            [SystemLanguage.Bulgarian] = IsoBulgarian,
            [SystemLanguage.Romanian] = IsoRomanian,
            [SystemLanguage.Slovak] = IsoSlovak,
            [SystemLanguage.Slovenian] = IsoSlovenian,
            [SystemLanguage.Ukrainian] = IsoUkrainian,
            [SystemLanguage.Lithuanian] = IsoLithuanian,
            [SystemLanguage.Latvian] = IsoLatvian,
            [SystemLanguage.Estonian] = IsoEstonian
        };

        private static readonly Dictionary<string, SystemLanguage> _isoToSystemLanguageMap = new()
        {
            [IsoEnglish] = SystemLanguage.English,
            [IsoJapanese] = SystemLanguage.Japanese,
            [IsoKorean] = SystemLanguage.Korean,
            [IsoChinese] = SystemLanguage.Chinese,
            [IsoChineseSimplified] = SystemLanguage.ChineseSimplified,
            [IsoChineseTraditional] = SystemLanguage.ChineseTraditional,
            [IsoSpanish] = SystemLanguage.Spanish,
            [IsoFrench] = SystemLanguage.French,
            [IsoGerman] = SystemLanguage.German,
            [IsoItalian] = SystemLanguage.Italian,
            [IsoPortuguese] = SystemLanguage.Portuguese,
            [IsoRussian] = SystemLanguage.Russian,
            [IsoArabic] = SystemLanguage.Arabic,
            [IsoHindi] = SystemLanguage.Hindi,
            [IsoThai] = SystemLanguage.Thai,
            [IsoVietnamese] = SystemLanguage.Vietnamese,
            [IsoTurkish] = SystemLanguage.Turkish,
            [IsoPolish] = SystemLanguage.Polish,
            [IsoDutch] = SystemLanguage.Dutch,
            [IsoSwedish] = SystemLanguage.Swedish,
            [IsoNorwegian] = SystemLanguage.Norwegian,
            [IsoDanish] = SystemLanguage.Danish,
            [IsoFinnish] = SystemLanguage.Finnish,
            [IsoCzech] = SystemLanguage.Czech,
            [IsoHungarian] = SystemLanguage.Hungarian,
            [IsoGreek] = SystemLanguage.Greek,
            [IsoHebrew] = SystemLanguage.Hebrew,
            [IsoBulgarian] = SystemLanguage.Bulgarian,
            [IsoRomanian] = SystemLanguage.Romanian,
            [IsoSlovak] = SystemLanguage.Slovak,
            [IsoSlovenian] = SystemLanguage.Slovenian,
            [IsoUkrainian] = SystemLanguage.Ukrainian,
            [IsoLithuanian] = SystemLanguage.Lithuanian,
            [IsoLatvian] = SystemLanguage.Latvian,
            [IsoEstonian] = SystemLanguage.Estonian
        };

        /// <summary>
        /// Converts a Unity SystemLanguage enum value to its corresponding ISO 639-1 language code.
        /// </summary>
        /// <param name="language">The SystemLanguage to convert.</param>
        /// <returns>
        /// The ISO 639-1 language code (e.g., "en" for English, "ja" for Japanese).
        /// Returns "en" (English) as the default if the language is not found in the mapping.
        /// </returns>
        [UsedImplicitly]
        internal static string SystemLanguageToISO(this SystemLanguage language)
            => _systemLanguageToISOMap.GetValueOrDefault(language, IsoEnglish);

        /// <summary>
        /// Converts an ISO 639-1 language code to its corresponding Unity SystemLanguage enum value.
        /// </summary>
        /// <param name="isoCode">The ISO 639-1 language code to convert (e.g., "en", "ja", "zh-CN").</param>
        /// <returns>
        /// The corresponding SystemLanguage enum value.
        /// Returns SystemLanguage.English as the default if the ISO code is not found in the mapping.
        /// </returns>
        [UsedImplicitly]
        internal static SystemLanguage ISOToSystemLanguage(this string isoCode)
            => _isoToSystemLanguageMap.GetValueOrDefault(isoCode, SystemLanguage.English);
    }
}