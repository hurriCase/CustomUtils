using System;
using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    [Resource(
        ResourcePaths.LocalizationKeyDatabaseFullPath,
        ResourcePaths.LocalizationKeyDatabaseAssetName,
        ResourcePaths.LocalizationKeyDatabaseResourcesPath
    )]
    internal sealed class LocalizationKeyDatabase : SingletonScriptableObject<LocalizationKeyDatabase>
    {
        [Serializable]
        internal sealed class KeyEntry
        {
            [field: SerializeField] internal string Guid { get; private set; }
            [field: SerializeField] internal string StringKey { get; private set; }
            [field: SerializeField] internal string SheetName { get; private set; }

            internal KeyEntry(string guid, string stringKey, string sheetName)
            {
                Guid = guid;
                StringKey = stringKey;
                SheetName = sheetName;
            }

#if UNITY_EDITOR
            internal void UpdateSheetName(string sheetName)
            {
                SheetName = sheetName;
            }

            internal void UpdateStringKey(string stringKey)
            {
                StringKey = stringKey;
            }
#endif
        }

        [SerializeField] private List<KeyEntry> _entries = new();

        private Dictionary<string, string> _guidToStringKey;
        private Dictionary<string, string> _stringKeyToGuid;
        private Dictionary<string, List<KeyEntry>> _sheetNameToKeys;

        internal void Initialize()
        {
            _guidToStringKey = new Dictionary<string, string>(_entries.Count);
            _stringKeyToGuid = new Dictionary<string, string>(_entries.Count);
            _sheetNameToKeys = new Dictionary<string, List<KeyEntry>>();

            foreach (var entry in _entries)
            {
                if (string.IsNullOrEmpty(entry.Guid) || string.IsNullOrEmpty(entry.StringKey))
                    continue;

                _guidToStringKey[entry.Guid] = entry.StringKey;
                _stringKeyToGuid[entry.StringKey] = entry.Guid;

                if (string.IsNullOrEmpty(entry.SheetName) is false)
                {
                    if (_sheetNameToKeys.ContainsKey(entry.SheetName) is false)
                        _sheetNameToKeys[entry.SheetName] = new List<KeyEntry>();

                    _sheetNameToKeys[entry.SheetName].Add(entry);
                }
            }
        }

        internal bool TryGetStringKey(string guid, out string stringKey) =>
            _guidToStringKey.TryGetValue(guid, out stringKey);

        internal bool TryGetGuid(string stringKey, out string guid) =>
            _stringKeyToGuid.TryGetValue(stringKey, out guid);

        internal string[] GetAllStringKeys() =>
            _entries.AsValueEnumerable()
                .Select(entry => entry.StringKey)
                .OrderBy(key => key)
                .ToArray();

        internal string[] GetStringKeysBySheet(string sheetName)
        {
            EnsureDictionariesInitialized();

            if (_sheetNameToKeys.TryGetValue(sheetName, out var keys) is false)
                return Array.Empty<string>();

            return keys.AsValueEnumerable()
                .Select(entry => entry.StringKey)
                .OrderBy(key => key)
                .ToArray();
        }

        internal string[] GetAllSheetNames()
        {
            EnsureDictionariesInitialized();

            return _sheetNameToKeys.Keys.AsValueEnumerable()
                .OrderBy(name => name)
                .ToArray();
        }

        internal bool TryGetSheetName(string stringKey, out string sheetName)
        {
            EnsureDictionariesInitialized();

            sheetName = null;

            if (_stringKeyToGuid.TryGetValue(stringKey, out var guid) is false)
                return false;

            var entry = _entries.AsValueEnumerable()
                .FirstOrDefault(e => e.Guid == guid);

            if (entry == null)
                return false;

            sheetName = entry.SheetName;
            return string.IsNullOrEmpty(sheetName) is false;
        }

        internal LocalizationKey[] GetAllKeys() =>
            _entries.AsValueEnumerable()
                .Select(entry => new LocalizationKey(entry.Guid))
                .ToArray();

#if UNITY_EDITOR
        internal void RegisterFromSheet(string guid, string stringKey)
        {
            EnsureDictionariesInitialized();

            if (_guidToStringKey.TryGetValue(guid, out var existingKey))
            {
                // GUID exists, update the string key if it changed
                if (existingKey != stringKey)
                {
                    var entry = _entries.AsValueEnumerable()
                        .FirstOrDefault(e => e.Guid == guid);

                    if (entry != null)
                    {
                        // Remove old string key mapping
                        _stringKeyToGuid.Remove(existingKey);

                        // Update to new string key
                        entry.UpdateStringKey(stringKey);
                        _guidToStringKey[guid] = stringKey;
                        _stringKeyToGuid[stringKey] = guid;

                        UnityEditor.EditorUtility.SetDirty(this);

                        Debug.Log($"[LocalizationKeyDatabase] Updated key: '{existingKey}' -> '{stringKey}' (GUID: {guid})");
                    }
                }
            }
            else
            {
                // New GUID from sheet
                CreateEntryFromSheet(guid, stringKey);
            }
        }

        private void CreateEntryFromSheet(string guid, string stringKey)
        {
            var entry = new KeyEntry(guid, stringKey, string.Empty);

            _entries.Add(entry);
            _guidToStringKey[guid] = stringKey;
            _stringKeyToGuid[stringKey] = guid;

            UnityEditor.EditorUtility.SetDirty(this);

            Debug.Log($"[LocalizationKeyDatabase] Registered new key from sheet: '{stringKey}' (GUID: {guid})");
        }

        internal LocalizationKey GetOrCreateKey(string stringKey, string sheetName = null)
        {
            EnsureDictionariesInitialized();

            if (_stringKeyToGuid.TryGetValue(stringKey, out var existingGuid))
            {
                if (string.IsNullOrEmpty(sheetName) is false)
                    UpdateSheetNameForKey(stringKey, sheetName);

                return new LocalizationKey(existingGuid);
            }

            var newGuid = Guid.NewGuid().ToString();
            var entry = new KeyEntry(newGuid, stringKey, sheetName ?? string.Empty);

            _entries.Add(entry);
            _guidToStringKey[newGuid] = stringKey;
            _stringKeyToGuid[stringKey] = newGuid;

            if (string.IsNullOrEmpty(sheetName) is false)
            {
                if (_sheetNameToKeys.ContainsKey(sheetName) is false)
                    _sheetNameToKeys[sheetName] = new List<KeyEntry>();

                _sheetNameToKeys[sheetName].Add(entry);
            }

            UnityEditor.EditorUtility.SetDirty(this);

            return new LocalizationKey(newGuid);
        }

        internal void UpdateSheetNameForKey(string stringKey, string sheetName)
        {
            EnsureDictionariesInitialized();

            var entry = _entries.AsValueEnumerable()
                .FirstOrDefault(e => e.StringKey == stringKey);

            if (entry == null)
                return;

            var oldSheetName = entry.SheetName;

            if (oldSheetName == sheetName)
                return;

            if (string.IsNullOrEmpty(oldSheetName) is false &&
                _sheetNameToKeys.TryGetValue(oldSheetName, out var oldList))
            {
                oldList.Remove(entry);
            }

            entry.UpdateSheetName(sheetName);

            if (string.IsNullOrEmpty(sheetName) is false)
            {
                if (_sheetNameToKeys.ContainsKey(sheetName) is false)
                    _sheetNameToKeys[sheetName] = new List<KeyEntry>();

                _sheetNameToKeys[sheetName].Add(entry);
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

        internal void RemoveUnusedKeys(HashSet<string> usedStringKeys)
        {
            EnsureDictionariesInitialized();

            var keysToRemove = _entries.AsValueEnumerable()
                .Where(entry => usedStringKeys.Contains(entry.StringKey) is false)
                .ToList();

            foreach (var entry in keysToRemove)
            {
                _entries.Remove(entry);
                _guidToStringKey.Remove(entry.Guid);
                _stringKeyToGuid.Remove(entry.StringKey);

                if (string.IsNullOrEmpty(entry.SheetName) is false &&
                    _sheetNameToKeys.TryGetValue(entry.SheetName, out var list))
                {
                    list.Remove(entry);
                }
            }

            if (keysToRemove.Count > 0)
                UnityEditor.EditorUtility.SetDirty(this);
        }

        private void EnsureDictionariesInitialized()
        {
            if (_guidToStringKey != null && _stringKeyToGuid != null && _sheetNameToKeys != null)
                return;

            Initialize();
        }
#endif
    }
}