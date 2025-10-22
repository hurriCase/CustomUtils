using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.AssetLoader;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Registry that stores all localization entries with GUID-based lookup.
    /// </summary>
    [Resource(
        ResourcePaths.LocalizationRegistryFullPath,
        ResourcePaths.LocalizationRegistryAssetName,
        ResourcePaths.LocalizationRegistryResourcesPath
    )]
    internal sealed class LocalizationRegistry : ScriptableObject
    {
        private static LocalizationRegistry _instance;

        [SerializeField] private List<LocalizationEntry> _entries = new();

        private Dictionary<string, LocalizationEntry> _guidLookup;
        private Dictionary<string, List<LocalizationEntry>> _tableLookup;

        internal static LocalizationRegistry Instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateInstance<LocalizationRegistry>();
                return _instance;
            }
        }

        internal IReadOnlyList<LocalizationEntry> Entries => _entries;

        internal void Initialize()
        {
            BuildLookupTables();
        }

        internal bool TryGetEntry(string guid, out LocalizationEntry entry)
        {
            if (_guidLookup == null)
                BuildLookupTables();

            return _guidLookup.TryGetValue(guid, out entry);
        }

        internal LocalizationEntry[] GetEntriesForTable(string tableName)
        {
            if (_tableLookup == null)
                BuildLookupTables();

            return _tableLookup.TryGetValue(tableName, out var entries)
                ? entries.ToArray()
                : System.Array.Empty<LocalizationEntry>();
        }

        internal string[] GetAllTableNames()
        {
            if (_tableLookup == null)
                BuildLookupTables();

            return _tableLookup.Keys.ToArray();
        }

        internal void AddOrUpdateEntry(LocalizationEntry entry)
        {
            var existingIndex = _entries.FindIndex(e => e.Guid == entry.Guid);

            if (existingIndex >= 0)
                _entries[existingIndex] = entry;
            else
                _entries.Add(entry);

            BuildLookupTables();
        }

        internal void RemoveEntry(string guid)
        {
            _entries.RemoveAll(e => e.Guid == guid);
            BuildLookupTables();
        }

        internal void Clear()
        {
            _entries.Clear();
            BuildLookupTables();
        }

        private void BuildLookupTables()
        {
            _guidLookup = new Dictionary<string, LocalizationEntry>();
            _tableLookup = new Dictionary<string, List<LocalizationEntry>>();

            foreach (var entry in _entries)
            {
                _guidLookup[entry.Guid] = entry;

                if (_tableLookup.ContainsKey(entry.TableName) is false)
                    _tableLookup[entry.TableName] = new List<LocalizationEntry>();

                _tableLookup[entry.TableName].Add(entry);
            }
        }

        internal LocalizationEntry[] SearchEntries(string searchText, string tableName = null)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return string.IsNullOrEmpty(tableName)
                    ? _entries.ToArray()
                    : GetEntriesForTable(tableName);
            }

            return _entries.AsValueEnumerable()
                .Where(e => (string.IsNullOrEmpty(tableName) || e.TableName == tableName) &&
                           (e.Key.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) ||
                            e.Guid.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }
    }
}