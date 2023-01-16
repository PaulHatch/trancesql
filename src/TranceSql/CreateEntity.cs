using System.Collections.Generic;
using System.Data.Common;

namespace TranceSql;

/// <summary>Defines a method to create an entity from a DbDataReader row.</summary>
/// <typeparam name="T">Entity type to create.</typeparam>
/// <param name="reader">An open DbDataReader instance to provide entity values.</param>
/// <param name="map">Map of columns-to-ordinal values for the current DbDataReader instance.</param>
/// <returns>A new entity based on the DbDataReader row.</returns>
public delegate T CreateEntity<T>(DbDataReader reader, IDictionary<string, int> map);