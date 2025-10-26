﻿using System;
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
        private static readonly List<SystemLanguage> _usedLanguages = new();

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
            _usedLanguages.Clear();

            foreach (var row in csvTable.Rows)
            {
                if (TryCreateEntryFromRow(row, sheetName, out var entry) is false)
                    continue;

                LocalizationRegistry.Instance.AddOrUpdateEntry(entry);
            }

            LocalizationRegistry.Instance.SupportedLanguages = _usedLanguages;
        }

        private static bool TryCreateEntryFromRow(CsvRow row, string sheetName, out LocalizationEntry localizationEntry)
        {
            localizationEntry = null;
            if (row.TryGetValue(GuidColumnName, out var guid) is false
                || row.TryGetValue(KeyColumnName, out var key) is false)
            {
                Debug.LogError("[LocalizationSheetProcessor::TryCreateEntryFromRow]" +
                               $"{GuidColumnName} or {KeyColumnName} column is missing in sheet '{sheetName}'");
                return false;
            }

            if (_processedGuids.Add(guid) is false)
            {
                Debug.LogError("[LocalizationSheetProcessor::CreateEntryFromRow]" +
                               $" Duplicate GUID '{guid}' in sheet '{sheetName}'");
                return false;
            }

            localizationEntry = new LocalizationEntry(guid, key, sheetName);

            foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
            {
                if (_usedLanguages.Contains(language) is false)
                    _usedLanguages.Add(language);

                if (row.TryGetValue(language.ToString(), out var translation))
                    localizationEntry.SetTranslation(language, translation);
            }

            return true;
        }
    }
}