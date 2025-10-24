﻿using System.Collections.Generic;
using CustomUtils.Runtime.Localization;
using Cysharp.Text;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal static class LocalizationSheetExporter
    {
        private const char Separator = '\t';

        internal static string ExportAllKeysWithGuids()
        {
            var entries = LocalizationRegistry.Instance.Entries.Values.ToArray();

            if (entries.Length == 0)
            {
                Debug.LogWarning("[LocalizationSheetExporter::ExportAllKeysWithGuids] No localization entries found.");
                return string.Empty;
            }

            using var tsvBuilder = ZString.CreateStringBuilder();

            tsvBuilder.Append("GUID");
            tsvBuilder.Append(Separator);
            tsvBuilder.AppendLine("Key");

            foreach (var entry in entries.OrderBy(static e => e.Key))
            {
                tsvBuilder.Append(entry.GUID);
                tsvBuilder.Append(Separator);
                tsvBuilder.AppendLine(EscapeField(entry.Key));
            }

            return tsvBuilder.ToString();
        }

        internal static string ExportSheet(string sheetName)
        {
            var entries = LocalizationRegistry.Instance.Entries.Values
                .Where(e => e.TableName == sheetName)
                .OrderBy(static e => e.Key)
                .ToArray();

            if (entries.Length == 0)
            {
                Debug.LogError($"[LocalizationSheetExporter::ExportSheet] No entries found for sheet '{sheetName}'.");
                return string.Empty;
            }

            var languages = GetLanguagesFromEntries(entries);

            using var tsvBuilder = ZString.CreateStringBuilder();

            tsvBuilder.Append("GUID");
            tsvBuilder.Append(Separator);
            tsvBuilder.Append("Key");

            foreach (var language in languages)
            {
                tsvBuilder.Append(Separator);
                tsvBuilder.Append(language.ToString());
            }

            tsvBuilder.AppendLine();

            foreach (var entry in entries)
            {
                tsvBuilder.Append(entry.GUID);
                tsvBuilder.Append(Separator);
                tsvBuilder.Append(EscapeField(entry.Key));

                foreach (var language in languages)
                {
                    tsvBuilder.Append(Separator);

                    if (entry.TryGetTranslation(language, out var translation))
                        tsvBuilder.Append(EscapeField(translation));
                }

                tsvBuilder.AppendLine();
            }

            return tsvBuilder.ToString();
        }

        private static List<SystemLanguage> GetLanguagesFromEntries(LocalizationEntry[] entries)
        {
            var languages = new HashSet<SystemLanguage>();

            foreach (var entry in entries)
            {
                foreach (var kvp in entry.Translations)
                    languages.Add(kvp.Key);
            }

            return languages.OrderBy(static l => l.ToString()).ToList();
        }

        private static string EscapeField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            if (field.Contains("\n") is false && field.Contains("\t") is false && field.Contains("\"") is false)
                return field;

            using var escaped = ZString.CreateStringBuilder();
            escaped.Append('"');

            foreach (var c in field)
            {
                if (c == '"')
                    escaped.Append("\"\"");
                else
                    escaped.Append(c);
            }

            escaped.Append('"');
            return escaped.ToString();
        }
    }
}