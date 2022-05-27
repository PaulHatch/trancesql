using System.Data.Common;

namespace TranceSql.Processing
{
    /// <summary>
    /// Result processor that uses a custom create entity delegate to generate a result.
    /// </summary>
    /// <typeparam name="TResult">Result type to return.</typeparam>
    internal class CustomResultProcessor<TResult> : IResultProcessor
    {
        private readonly CreateEntity<TResult> _valueProvider;

        /// <summary>
        /// Result processor that returns a string/object dictionary for a single row of a result
        /// where each column is an entry.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        public CustomResultProcessor(CreateEntity<TResult> valueProvider)
        {
            _valueProvider = valueProvider;
        }
        /// <summary>Processes the command and generates a custom result.</summary>
        /// <param name="reader">An open data reader queued to the appropriate result set.</param>
        /// <returns>The result of the processor for this query.</returns>
        public object? Process(DbDataReader reader)
        {
            var map = EntityMapping.MapDbReaderColumns(reader);
            return _valueProvider(reader, map);
        }
    }
}
