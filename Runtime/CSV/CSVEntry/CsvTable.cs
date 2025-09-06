using JetBrains.Annotations;

namespace CustomUtils.Runtime.CSV.CSVEntry
{
    /// <summary>
    /// Represents a parsed CSV document containing an array of rows.
    /// </summary>
    [UsedImplicitly]
    public readonly struct CsvTable
    {
        /// <summary>
        /// Gets the collection of rows in this CSV document.
        /// </summary>
        [UsedImplicitly]
        internal CsvRow[] Rows { get; }

        internal CsvTable(CsvRow[] rows)
        {
            Rows = rows;
        }
    }
}