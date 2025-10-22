using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Represents a single localization entry with GUID, key, and translations.
    /// </summary>
    [Serializable]
    internal sealed class LocalizationEntry
    {
        [SerializeField] private string _guid;
        [SerializeField] private string _key;
        [SerializeField] private string _tableName;
        [SerializeField] private Dictionary<SystemLanguage, string> _translations;

        internal string Guid => _guid;
        internal string Key => _key;
        internal string TableName => _tableName;
        internal IReadOnlyDictionary<SystemLanguage, string> Translations => _translations;

        internal LocalizationEntry(string guid, string key, string tableName)
        {
            _guid = guid;
            _key = key;
            _tableName = tableName;
            _translations = new Dictionary<SystemLanguage, string>();
        }

        internal void SetTranslation(SystemLanguage language, string translation)
        {
            _translations[language] = translation;
        }

        internal bool TryGetTranslation(SystemLanguage language, out string translation) =>
            _translations.TryGetValue(language, out translation);

        internal bool HasTranslation(SystemLanguage language) =>
            _translations.ContainsKey(language) &&
            string.IsNullOrEmpty(_translations[language]) is false;
    }
}