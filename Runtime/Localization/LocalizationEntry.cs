using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Represents a single localization entry with GUID, key, and translations.
    /// </summary>
    [Serializable]
    internal sealed class LocalizationEntry
    {
        [field: SerializeField, InspectorReadOnly] internal string Key { get; private set; }
        [field: SerializeField, InspectorReadOnly] internal string Guid { get; private set; }
        [field: SerializeField, InspectorReadOnly] internal string TableName { get; private set; }

        [SerializeField, InspectorReadOnly] private List<SystemLanguage> _languages = new();
        [SerializeField, InspectorReadOnly] private List<string> _translations = new();

        private Dictionary<SystemLanguage, string> _translationLookup;

        internal LocalizationEntry(string guid, string key, string tableName)
        {
            Guid = guid;
            Key = key;
            TableName = tableName;
        }

        internal void SetTranslation(SystemLanguage language, string translation)
        {
            var index = _languages.IndexOf(language);

            if (index >= 0)
                _translations[index] = translation;
            else
            {
                _languages.Add(language);
                _translations.Add(translation);
            }

            _translationLookup = null;
        }

        internal bool TryGetTranslation(SystemLanguage language, out string translation)
        {
            BuildLookupIfNeeded();
            return _translationLookup.TryGetValue(language, out translation);
        }

        internal bool HasTranslation(SystemLanguage language)
        {
            BuildLookupIfNeeded();
            return _translationLookup.ContainsKey(language) &&
                   string.IsNullOrEmpty(_translationLookup[language]) is false;
        }

        private void BuildLookupIfNeeded()
        {
            if (_translationLookup != null)
                return;

            _translationLookup = new Dictionary<SystemLanguage, string>();

            for (var i = 0; i < _languages.Count && i < _translations.Count; i++)
                _translationLookup[_languages[i]] = _translations[i];
        }
    }
}