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

        [field: SerializeField]
        internal SerializedDictionary<string, HashSet<string>> TableToGuids { get; private set; } = new();

        [field: SerializeField] public List<SystemLanguage> SupportedLanguages { get; set; } = new();

        internal void AddOrUpdateEntry(LocalizationEntry entry)
        {
            Entries[entry.GUID] = entry;

            if (TableToGuids.ContainsKey(entry.TableName) is false)
                TableToGuids[entry.TableName] = new HashSet<string>();

            TableToGuids[entry.TableName].Add(entry.GUID);

            this.MarkAsDirty();
        }

        internal IList SearchEntries(string searchText, string tableName = null)
        {
            if (string.IsNullOrEmpty(searchText))
                return Entries.Values.ToArray();

            var entriesToSearch = string.IsNullOrEmpty(tableName)
                ? Entries.Values
                : GetEntriesForTable(tableName);

            return entriesToSearch
                .Where(entry => entry.Key.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        internal void Clear()
        {
            Entries.Clear();
            TableToGuids.Clear();
            this.MarkAsDirty();
        }

        private IReadOnlyCollection<LocalizationEntry> GetEntriesForTable(string tableName) =>
            TableToGuids.TryGetValue(tableName, out var guids)
                ? guids.Select(guid => Entries[guid]).ToArray()
                : Array.Empty<LocalizationEntry>();
    }
}