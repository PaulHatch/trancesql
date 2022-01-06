using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace TranceSql.Processing
{
    /// <summary>
    /// Result processor that returns a single class with one or more properties assigned to
    /// the results of the query.
    /// </summary>
    /// <typeparam name="TResult">Type to return.</typeparam>
    internal class MappedResultProcessor<TResult> : IResultProcessor
        where TResult : new()
    {
        // properties to assign
        private IEnumerable<Tuple<PropertyInfo, Type>> _map;
        // generic methods for our entity mapping helper methods
        private static readonly MethodInfo _readData = typeof(EntityMappingHelper).GetTypeInfo().GetMethod(nameof(EntityMappingHelper.ReadData), new[] { typeof(DbDataReader) });
        private static readonly MethodInfo _createInstance = typeof(EntityMappingHelper).GetTypeInfo().GetMethod(nameof(EntityMappingHelper.CreateInstance), new[] { typeof(DbDataReader) });

        /// <summary>
        /// Result processor that returns a single class with one or more properties assigned to
        /// the results of the query.
        /// </summary>
        /// <param name="map">The map of properties to assign.</param>    
        public MappedResultProcessor(IEnumerable<Tuple<PropertyInfo, Type>> map)
        {
            _map = map;
        }

        /// <summary>
        /// Processes the result as a mapped result.
        /// </summary>
        /// <param name="reader">An open data reader queued to the appropriate result set.</param>
        /// <returns>The result for this query.</returns>
        public object Process(DbDataReader reader)
        {
            TResult result = new TResult();

            // Map the properties
            foreach (var mappedProperty in _map)
            {
                if (mappedProperty.Item1.PropertyType.GetTypeInfo().IsGenericType &&
                   mappedProperty.Item1.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    // lists
                    var genericReadData = _readData.MakeGenericMethod(mappedProperty.Item2);
                    var collectionResults = genericReadData.Invoke(null, new object[] { reader });
                    mappedProperty.Item1.SetValue(result, collectionResults);
                }
                else
                {
                    // individual result
                    var genericCreateInstance = _createInstance.MakeGenericMethod(mappedProperty.Item2);
                    var instanceResult = genericCreateInstance.Invoke(null, new object[] { reader });
                    mappedProperty.Item1.SetValue(result, instanceResult);
                }

                reader.NextResult();
            }

            return result;
        }
    }
}
