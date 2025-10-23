using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using CustomUtils.Runtime.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Serializable]
    internal sealed class LocalizationEntry
    {
        [field: SerializeField, InspectorReadOnly] internal string Key { get; private set; }
        [field: SerializeField, InspectorReadOnly] internal string Guid { get; private set; }
        [field: SerializeField, InspectorReadOnly] internal string TableName { get; private set; }

        [field: SerializeField, InspectorReadOnly]
        internal SerializedDictionary<SystemLanguage, string> Translations { get; private set; } = new();

        internal LocalizationEntry(string guid, string key, string tableName)
        {
            Guid = guid;
            Key = key;
            TableName = tableName;
        }

        internal void SetTranslation(SystemLanguage language, string translation)
        {
            Translations[language] = translation;
        }

        internal bool TryGetTranslation(SystemLanguage language, out string translation)
            => Translations.TryGetValue(language, out translation);

        internal bool HasTranslation(SystemLanguage language) => Translations.ContainsKey(language);
    }
}