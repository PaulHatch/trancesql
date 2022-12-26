using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace TranceSql.Processing
{
    internal class SingleResultProcessor
    {
        internal static readonly MethodInfo ReadData = typeof(EntityMappingHelper)
            .GetMethod(nameof(EntityMappingHelper.ReadData))!;
    }
    
    /// <summary>
    /// Result processor that returns a single row as the specified type. Addition properties
    /// can be populated from additional result sets returned by the query.
    /// </summary>
    internal class SingleResultProcessor<TResult> : IResultProcessor
    {
        private readonly TResult? _defaultResult;
        private readonly IEnumerable<PropertyInfo>? _properties;
        
        /// <summary>
        /// Result processor that returns a single row as the specified type. Addition properties
        /// can be populated from additional result sets returned by the query.
        /// </summary>
        /// <param name="defaultResult">The default result to return if the result is empty.</param>
        /// <param name="properties">Additional properties to populate from subsequent result sets.</param>
        public SingleResultProcessor(TResult? defaultResult, IEnumerable<PropertyInfo>? properties)
        {
            _defaultResult = defaultResult;
            _properties = properties;
        }

        /// <summary>
        /// Processes the result as a single entity result.
        /// </summary>
        /// <param name="reader">An open data reader queued to the appropriate result set.</param>
        /// <returns>The result for this query.</returns>
        public object? Process(DbDataReader reader)
        {
            if (EntityMapping.IsSimpleType<TResult>())
            {
                // requested type can be mapped directly to a CLR type

                if (reader.Read())
                {
                    return EntityMapping.ReadHelper.Get(reader, 0, _defaultResult);
                }

                return _defaultResult;
            }
            
            // else if requested type must be mapped from the result row

            var result = reader.CreateInstance(_defaultResult);

            // Populate collections
            if (_properties != null)
            {
                foreach (var collection in _properties)
                {
                    if (!reader.NextResult())
                    {
                        throw new InvalidOperationException("Not enough result sets were returned by the query to assign all the requested properties.");
                    }

                    // Don't try to assign the result if it is null
                    if (result == null) continue;
                    
                    var genericReadData = SingleResultProcessor.ReadData.MakeGenericMethod(collection.PropertyType.GetCollectionType() ?? typeof(object));
                    var collectionResults = genericReadData.Invoke(null, new object[] { reader });
                    collection.SetValue(result, collectionResults);
                }
            }

            return result;
        }
    }
}
