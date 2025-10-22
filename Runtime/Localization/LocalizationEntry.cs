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
        [SerializeField, InspectorReadOnly] private string _key;
        [SerializeField, InspectorReadOnly] private string _guid;
        [SerializeField, InspectorReadOnly] private string _tableName;
        [SerializeField, InspectorReadOnly] private List<SystemLanguage> _languages = new();
        [SerializeField, InspectorReadOnly] private List<string> _translations = new();

        private Dictionary<SystemLanguage, string> _translationLookup;

        internal string Guid => _guid;
        internal string Key => _key;
        internal string TableName => _tableName;

        internal LocalizationEntry(string guid, string key, string tableName)
        {
            _guid = guid;
            _key = key;
            _tableName = tableName;
        }

        internal void SetTranslation(SystemLanguage language, string translation)
        {
            var index = _languages.IndexOf(language);

            if (index >= 0)
            {
                _translations[index] = translation;
            }
            else
            {
                _languages.Add(language);
                _translations.Add(translation);
            }

            // Clear lookup cache to rebuild on next access
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