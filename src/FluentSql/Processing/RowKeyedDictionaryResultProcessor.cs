using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql.Processing
{
    /// <summary>
    /// Result processor that returns a dictionary for a the first two columns of a result
    /// where each key/value pair is a row.
    /// </summary>
    /// <typeparam name="TKey">The type of the key, e.g. the first column type.</typeparam>
    /// <typeparam name="TValue">The type of the value, e.g. the second column type.</typeparam>
    internal class RowKeyedDictionaryResultProcessor<TKey,TValue> : IResultProcessor
    {
        /// <summary>
        /// Processes the result as a row-keyed dictionary.
        /// </summary>
        /// <param name="reader">An open data reader queued to the appropriate result set.</param>
        /// <returns>The result for this query.</returns>
        public object Process(DbDataReader reader)
        {
            return reader.CreateRowKeyedDictionary<TKey, TValue>();
        }
    }
}
