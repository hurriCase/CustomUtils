using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.CSV.CSVEntry
{
    /// <summary>
    /// Represents a single row in a CSV document with column-based value access.
    /// </summary>
    [UsedImplicitly]
    public readonly struct CsvRow
    {
        private readonly Dictionary<string, int> _columnMap;
        private readonly string[] _values;

        internal CsvRow(string[] values, IReadOnlyList<string> columnNames)
        {
            _values = values;

            _columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < columnNames.Count; i++)
                _columnMap[columnNames[i]] = i;
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified column name in the current CSV row.
        /// </summary>
        /// <param name="columnName">The name of the column to retrieve the value from (case-insensitive).</param>
        /// <param name="value">When this method returns,
        /// contains the value associated with the specified column if the operation succeeds,
        /// or an empty string if it fails.</param>
        /// <returns>true if the value is successfully retrieved; otherwise, false.</returns>
        [UsedImplicitly]
        public bool TryGetValue(string columnName, out string value)
        {
            value = string.Empty;
            if (_columnMap.TryGetValue(columnName, out var index) is false || index >= _values.Length)
                return false;

            value = _values[index];
            return value != null;
        }

        /// <summary>
        /// Gets the first value from a column whose name matches the specified regex pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to match against column names (case-insensitive).</param>
        /// <returns>The first matching value, or empty string if no column matches the pattern.</returns>
        [UsedImplicitly]
        public string GetValueByPattern(string pattern)
        {
            foreach (var (headerName, index) in _columnMap)
            {
                if (Regex.IsMatch(headerName, pattern, RegexOptions.IgnoreCase) is false)
                    continue;

                if (index < _values.Length)
                    return _values[index] ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets all values from columns whose names match the specified regex pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to match against column names (case-insensitive).</param>
        /// <returns>A list of all values from columns that match the pattern. Only includes valid (non-null, non-empty) values.</returns>
        [UsedImplicitly]
        public List<string> GetValuesByPattern(string pattern)
        {
            var result = new List<string>();

            foreach (var (headerName, index) in _columnMap)
            {
                if (Regex.IsMatch(headerName, pattern, RegexOptions.IgnoreCase) is false)
                    continue;

                if (index < _values.Length && string.IsNullOrEmpty(_values[index]) is false)
                    result.Add(_values[index]);
            }

            return result;
        }
    }
}