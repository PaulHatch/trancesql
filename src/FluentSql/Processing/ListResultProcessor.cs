using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql.Processing
{
    /// <summary>
    /// Result processor that creates a list of items of the specified type.
    /// </summary>
    /// <typeparam name="TResult">The entity type of result.</typeparam>
    internal class ListResultProcessor<TResult> : IResultProcessor
    {
        /// <summary>
        /// Processes the result as a list result.
        /// </summary>
        /// <param name="reader">An open data reader queued to the appropriate result set.</param>
        /// <returns>The result for this query.</returns>
        public object Process(DbDataReader reader)
        {
            return reader.ReadData<TResult>();
        }
    }
}
