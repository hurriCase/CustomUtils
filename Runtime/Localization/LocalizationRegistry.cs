using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Registry that stores all localization entries with GUID-based lookup.
    /// Persists data between sessions as a ScriptableObject asset.
    /// </summary>
    [Resource(
        ResourcePaths.LocalizationSettingsFullPath,
        ResourcePaths.LocalizationRegistryAssetName,
        ResourcePaths.LocalizationSettingsResourcesPath
    )]
    internal sealed class LocalizationRegistry : SingletonScriptableObject<LocalizationRegistry>
    {
        private static LocalizationRegistry _instance;

        [SerializeField] private List<LocalizationEntry> _entries = new();

        private Dictionary<string, LocalizationEntry> _guidLookup = new();
        private Dictionary<string, List<LocalizationEntry>> _tableLookup = new();
        private bool _isInitialized;

        internal IReadOnlyList<LocalizationEntry> Entries => _entries;

        internal void Initialize()
        {
            BuildLookupTables();
            _isInitialized = true;
        }

        internal bool TryGetEntry(string guid, out LocalizationEntry entry)
        {
            if (_isInitialized is false)
                Initialize();

            return _guidLookup.TryGetValue(guid, out entry);
        }

        internal LocalizationEntry[] GetEntriesForTable(string tableName)
        {
            if (_isInitialized is false)
                Initialize();

            return _tableLookup.TryGetValue(tableName, out var entries)
                ? entries.ToArray()
                : System.Array.Empty<LocalizationEntry>();
        }

        internal string[] GetAllTableNames()
        {
            if (_isInitialized is false)
                Initialize();

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

        [ContextMenu("Build Lookup Tables")]
        private void BuildLookupTables()
        {
            _guidLookup.Clear();
            _tableLookup.Clear();

            foreach (var entry in _entries)
            {
                _guidLookup[entry.Guid] = entry;

                if (_tableLookup.ContainsKey(entry.TableName) is false)
                    _tableLookup[entry.TableName] = new List<LocalizationEntry>();

                _tableLookup[entry.TableName].Add(entry);
            }

            this.MarkAsDirty();
        }

        internal LocalizationEntry[] SearchEntries(string searchText, string tableName = null)
        {
            if (_isInitialized is false)
                Initialize();

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