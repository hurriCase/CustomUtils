using System;
using System.Collections.Generic;
using CustomUtils.Runtime.CSV.CSVEntry;
using Cysharp.Text;
using ZLinq;

namespace CustomUtils.Runtime.CSV
{
    internal sealed class CsvParser
    {
        private const char Quote = '"';
        private const char Comma = ',';

        internal CsvTable Parse(string csvContent)
        {
            if (TryGetLines(csvContent, out var lines) is false)
                return new CsvTable(Array.Empty<CsvRow>());

            // Parse header
            var headerValues = ParseLine(lines[0]);

            var rows = ParseRows(lines, headerValues);

            return new CsvTable(rows);
        }

        private bool TryGetLines(string csvContent, out string[] lines)
        {
            lines = null;
            if (string.IsNullOrWhiteSpace(csvContent))
                return false;

            lines = csvContent.Split('\n')
                .Where(static line => string.IsNullOrWhiteSpace(line) is false)
                .Select(static line => line.Trim())
                .ToArray();

            return lines.Length > 1;
        }

        private CsvRow[] ParseRows(IReadOnlyList<string> lines, IReadOnlyList<string> header)
        {
            // skip the first header row
            var rows = new CsvRow[lines.Count - 1];
            for (var i = 1; i < lines.Count; i++)
            {
                var rowValues = ParseLine(lines[i]);
                rows[i - 1] = new CsvRow(rowValues, header);
            }

            return rows;
        }

        private string[] ParseLine(string line)
        {
            var values = new List<string>();
            var inQuotes = false;

            using var valueBuilder = ZString.CreateStringBuilder(false);

            foreach (var character in line)
            {
                switch (character)
                {
                    case Quote:
                        inQuotes = inQuotes is false;
                        break;

                    case Comma when inQuotes is false:
                        values.Add(valueBuilder.ToString().Trim());
                        valueBuilder.Clear();
                        break;

                    default:
                        valueBuilder.Append(character);
                        break;
                }
            }

            values.Add(valueBuilder.ToString().Trim());

            return values.ToArray();
        }
    }
}