using System.Data.Common;

namespace TranceSql.Processing
{
    /// <summary>
    /// Defines a processor for a specific type of SQL result expected from a data reader. Implementations
    /// receive an open data reader and should return the expected result.
    /// </summary>
    internal interface IResultProcessor
    {
        /// <summary>
        /// Processes the result on a given command.
        /// </summary>
        /// <param name="reader">An open data reader queued to the appropriate result set.</param>
        /// <returns>The result of the processor for this query.</returns>
        object? Process(DbDataReader reader);
    }
}
