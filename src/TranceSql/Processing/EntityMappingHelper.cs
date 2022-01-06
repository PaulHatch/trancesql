using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace TranceSql.Processing
{
    /// <summary>
    /// Helper methods for entity mapping.
    /// </summary>
    internal static class EntityMappingHelper
    {
        /// <summary>
        /// Creates an enumerable list of instances of the specified type create from 
        /// a data reader.
        /// </summary>
        /// <typeparam name="T">
        /// The data object type. This should be a class with properties matching the reader
        /// value.
        /// </typeparam>
        /// <param name="reader">An open reader ready to be read to a list.</param>
        /// <returns>An enumerable list of instances of type 'T' created from the data reader.</returns>
        public static IEnumerable<T> ReadData<T>(this DbDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var result = new List<T>();

            if (EntityMapping.IsSimpleType<T>())
            {
                while (reader.Read())
                {
                    result.Add(EntityMapping.ReadHelper.Get<T>(reader, 0));
                }
            }
            else
            {
                var map = EntityMapping.MapDbReaderColumns(reader);
                var readEntity = EntityMapping.GetEntityFunc<T>();

                while (reader.Read())
                {
                    result.Add(readEntity(reader, map));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates a dictionary from the rows of two columns of a data reader.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The key type.</typeparam>
        /// <param name="reader">An open reader ready to be read to a list.</param>
        /// <returns>A dictionary created from the first two columns of the data reader.</returns>
        public static IDictionary<TKey, TValue> CreateRowKeyedDictionary<TKey, TValue>(this DbDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (!EntityMapping.IsSimpleType<TKey>())
            {
                throw new ArgumentException("The key type of the dictionary must be a simple type.");
            }
            if (!EntityMapping.IsSimpleType<TValue>())
            {
                throw new ArgumentException("The value type of the dictionary must be a simple type.");
            }

            var result = new Dictionary<TKey, TValue>();

            while (reader.Read())
            {
                result.Add(EntityMapping.ReadHelper.Get<TKey>(reader, 0), EntityMapping.ReadHelper.Get<TValue>(reader, 1));
            }

            return result;
        }

        /// <summary>
        /// Creates a dictionary from a single row of a data reader, setting the dictionary key to the column names
        /// and the value to the values from the row.
        /// </summary>
        /// <param name="reader">An open reader with at least one row result (subsequent rows are ignored.)</param>
        /// <param name="columns">The columns to read. If this value is null, all columns will be returned.</param>
        /// <returns>A dictionary created from the first row of the data reader.</returns>
        /// <remarks>Keep in mind that result may contain DbNull type values.</remarks>
        public static IDictionary<string, object> CreateColumnKeyedDictionary(this DbDataReader reader, IEnumerable<string> columns)
        {
            if (reader.Read())
            {
                if (columns?.Any() != true)
                {
                    var map = EntityMapping.MapDbReaderColumns(reader);
                    return map.ToDictionary(k => k.Key, v => reader[v.Value]);
                }
                else
                {
                    return columns.ToDictionary(k => k, v => reader[v]);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of the specified type from a data reader.
        /// </summary>
        /// <typeparam name="T">Data object type.</typeparam>
        /// <param name="reader">Instance of an open data reader.</param>
        /// <returns>A new instance of type 'T' initialized to the value of the current row.</returns>
        public static T CreateInstance<T>(this DbDataReader reader)
        {
            return reader.CreateInstance<T>(default);
        }

        /// <summary>
        /// Creates an instance of the specified type from a data reader.
        /// </summary>
        /// <typeparam name="T">Data object type.</typeparam>
        /// <param name="reader">Instance of an open data reader.</param>
        /// <param name="defaultResult">The default result if the data reader has no rows.</param>
        /// <returns>A new instance of type 'T' initialized to the value of the current row.</returns>
        public static T CreateInstance<T>(this DbDataReader reader, T defaultResult)
        {
            // if there are no rows return the specified default
            if (reader.Read())
            {
                if (EntityMapping.IsSimpleType<T>())
                {
                    return EntityMapping.ReadHelper.Get(reader, 0, defaultResult);
                }
                else
                {
                    var map = EntityMapping.MapDbReaderColumns(reader);
                    var readEntity = EntityMapping.GetEntityFunc<T>();

                    return readEntity(reader, map);
                }
            }
            else
            {
                return defaultResult;
            }
        }
    }
}
