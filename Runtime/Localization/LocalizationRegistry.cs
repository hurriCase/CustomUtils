using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    [Resource(
        ResourcePaths.LocalizationSettingsFullPath,
        ResourcePaths.LocalizationRegistryAssetName,
        ResourcePaths.LocalizationSettingsResourcesPath
    )]
    public sealed class LocalizationRegistry : SingletonScriptableObject<LocalizationRegistry>
    {
        [field: SerializeField]
        internal SerializedDictionary<string, LocalizationEntry> Entries { get; private set; } = new();
        [field: SerializeField] public List<SystemLanguage> SupportedLanguages { get; set; } = new();

        internal IReadOnlyDictionary<string, HashSet<string>> TableToGuids => _tableToGuids;

        private readonly Dictionary<string, HashSet<string>> _tableToGuids = new();

        internal void AddOrUpdateEntry(LocalizationEntry entry)
        {
            Entries[entry.GUID] = entry;

            if (_tableToGuids.ContainsKey(entry.TableName) is false)
                _tableToGuids[entry.TableName] = new HashSet<string>();

            _tableToGuids[entry.TableName].Add(entry.GUID);

            this.MarkAsDirty();
        }

        internal IList SearchEntries(string searchText, string tableName = null)
        {
            var entriesToSearch = string.IsNullOrEmpty(tableName)
                ? Entries.Values
                : GetEntriesForTable(tableName);

            if (string.IsNullOrEmpty(searchText))
                return entriesToSearch.ToArray();

            return entriesToSearch
                .Where(entry => entry.Key.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        internal void Clear()
        {
            Entries.Clear();
            _tableToGuids.Clear();
            this.MarkAsDirty();
        }

        private IReadOnlyCollection<LocalizationEntry> GetEntriesForTable(string tableName) =>
            _tableToGuids.TryGetValue(tableName, out var guids)
                ? guids.Select(guid => Entries[guid]).ToArray()
                : Array.Empty<LocalizationEntry>();
    }
}