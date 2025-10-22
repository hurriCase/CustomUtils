using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomUtils.Runtime.Localization;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal static class LocalizationGuidGenerator
    {
        internal static string GenerateGuidsForExistingKeys()
        {
            var allKeys = LocalizationController.GetAllKeys();
            if (allKeys == null || allKeys.Length == 0)
            {
                Debug.LogWarning("[LocalizationGuidGenerator] No localization keys found.");
                return string.Empty;
            }

            var guidMapping = new Dictionary<string, string>();

            foreach (var key in allKeys)
            {
                var guid = GenerateGuid();
                guidMapping[key] = guid;
            }

            return CreateCsvOutput(guidMapping);
        }

        internal static string GenerateGuidsForSheet(string sheetName)
        {
            var settings = LocalizationDatabase.Instance;
            var sheet = settings.Sheets.FirstOrDefault(s => s.Name == sheetName);

            if (!sheet?.TextAsset)
            {
                Debug.LogError($"[LocalizationGuidGenerator] Sheet '{sheetName}' not found or has no TextAsset.");
                return string.Empty;
            }

            var lines = LocalizationCsvParser.ParseLines(sheet.TextAsset.text);
            if (lines.Count == 0)
                return string.Empty;

            var headerColumns = LocalizationCsvParser.ParseColumns(lines[0]);
            var languages = ParseLanguages(headerColumns);

            var csvBuilder = new StringBuilder();

            csvBuilder.Append("GUID,Key");
            foreach (var language in languages)
                csvBuilder.Append($",{language}");
            csvBuilder.AppendLine();

            for (var i = 1; i < lines.Count; i++)
            {
                var columns = LocalizationCsvParser.ParseColumns(lines[i]);
                if (columns.Count == 0 || string.IsNullOrEmpty(columns[0]))
                    continue;

                var guid = GenerateGuid();
                var key = EscapeCsvField(columns[0]);

                csvBuilder.Append($"{guid},{key}");

                for (var j = 1; j < columns.Count && j - 1 < languages.Count; j++)
                {
                    var translation = EscapeCsvField(columns[j]);
                    csvBuilder.Append($",{translation}");
                }

                csvBuilder.AppendLine();
            }

            return csvBuilder.ToString();
        }

        private static List<SystemLanguage> ParseLanguages(List<string> headerColumns)
        {
            var languages = new List<SystemLanguage>();

            for (var i = 1; i < headerColumns.Count; i++)
            {
                if (Enum.TryParse<SystemLanguage>(headerColumns[i], true, out var language))
                    languages.Add(language);
            }

            return languages;
        }

        private static string CreateCsvOutput(Dictionary<string, string> guidMapping)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("GUID,Key");

            foreach (var kvp in guidMapping.AsValueEnumerable().OrderBy(static x => x.Key))
                csvBuilder.AppendLine($"{kvp.Value},{EscapeCsvField(kvp.Key)}");

            return csvBuilder.ToString();
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return field;

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                return $"\"{field.Replace("\"", "\"\"")}\"";

            return field;
        }

        private static string GenerateGuid() => Guid.NewGuid().ToString("N");
    }
}