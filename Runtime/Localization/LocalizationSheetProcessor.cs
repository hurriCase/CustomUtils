using System;
using System.Collections.Generic;
using CustomUtils.Runtime.CSV;
using CustomUtils.Runtime.CSV.CSVEntry;
using CustomUtils.Runtime.Downloader;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    internal static class LocalizationSheetProcessor
    {
        private const string GuidColumnName = "GUID";
        private const string KeyColumnName = "Key";

        private static readonly HashSet<string> _processedGuids = new();

        internal static void ProcessSheets(List<Sheet> sheets)
        {
            LocalizationRegistry.Instance.Clear();

            _processedGuids.Clear();

            foreach (var sheet in sheets)
            {
                if (!sheet?.TextAsset)
                {
                    Debug.LogWarning("[LocalizationSheetProcessor::ProcessSheets]" +
                                     $" Sheet '{sheet?.Name}' has no TextAsset");
                    continue;
                }

                var csvTable = CsvParser.Parse(sheet.TextAsset.text);
                ProcessSheet(csvTable, sheet.Name);
            }
        }

        private static void ProcessSheet(CsvTable csvTable, string sheetName)
        {
            foreach (var row in csvTable.Rows)
            {
                var entry = CreateEntryFromRow(row, sheetName);

                if (entry is null)
                    continue;

                LocalizationRegistry.Instance.AddOrUpdateEntry(entry);
            }
        }

        private static LocalizationEntry CreateEntryFromRow(CsvRow row, string sheetName)
        {
            var guid = row.GetValue(GuidColumnName);
            var key = row.GetValue(KeyColumnName);

            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(key))
                return null;

            if (_processedGuids.Add(guid) is false)
            {
                Debug.LogError("[LocalizationSheetProcessor::CreateEntryFromRow]" +
                               $" Duplicate GUID '{guid}' in sheet '{sheetName}'");
                return null;
            }

            var entry = new LocalizationEntry(guid, key, sheetName);

            foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
            {
                var translation = row.GetValue(language.ToString());

                if (string.IsNullOrEmpty(translation) is false)
                    entry.SetTranslation(language, translation);
            }

            return entry;
        }
    }
}