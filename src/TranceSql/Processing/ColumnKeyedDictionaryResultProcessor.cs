using System.Collections.Generic;
using System.Data.Common;

namespace TranceSql.Processing;

/// <summary>
/// Result processor that returns a string/object dictionary for a single row of a result
/// where each column is an entry.
/// </summary>
internal class ColumnKeyedDictionaryResultProcessor : IResultProcessor
{
    private readonly IEnumerable<string> _columns;

    /// <summary>
    /// Result processor that returns the a string/object dictionary for a single row of a result
    /// where each column is an entry.
    /// </summary>
    /// <param name="columns">The columns to read. If this value is null, all columns will be returned.</param>
    public ColumnKeyedDictionaryResultProcessor(IEnumerable<string> columns)
    {
        _columns = columns;
    }

    /// <summary>
    /// Processes the result as a column-keyed dictionary.
    /// </summary>
    /// <param name="reader">An open data reader queued to the appropriate result set.</param>
    /// <returns>The result for this query.</returns>
    public object? Process(DbDataReader reader)
    {
        return reader.CreateColumnKeyedDictionary(_columns);
    }
}