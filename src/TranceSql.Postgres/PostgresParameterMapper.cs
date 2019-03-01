using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Configurable implementation of <see cref="IParameterMapper"/>
    /// for handling Postgres parameters. When providing values, types not in the
    /// "System" namespace will be specified as JSON or JSONB (as configured).
    /// </summary>
    public class PostgresParameterMapper : DefaultParameterMapper
    {
        /// <summary>
        /// Gets or sets a value indicating whether to set the parameter type to
        /// JSONB when automatically assigning the parameter type for unknown values.
        /// If true JSONB will be used, if false, JSON will be used instead.
        /// </summary>
        public bool UseJsonb { get; set;  } = true;

        /// <summary>
        /// Gets the custom type map. When the type specified in the map is
        /// provided to a command, the corresponding type name or DB type will
        /// be used as the Postgres type name.
        /// </summary>
        public CustomTypeMap CustomTypes { get; }
            = new CustomTypeMap();

        /// <summary>
        /// Map of custom Postgres types.
        /// </summary>
        public class CustomTypeMap : IEnumerable
        {
            private readonly IDictionary<Type, (object type, Func<object, object> extractor)> _values
                = new Dictionary<Type, (object type, Func<object, object> extractor)>();

            internal bool TryGetMapping(Type type, out object dbType, out Func<object, object> extractor)
            {
                if (_values.ContainsKey(type))
                {
                    (dbType, extractor) = _values[type];
                    return true;
                }
                dbType = null;
                extractor = null;
                return false;
            }

            /// <summary>
            /// Adds the specified type to this map.
            /// </summary>
            /// <param name="type">The .NET type to map.</param>
            /// <param name="dbType">The name of the Postgres type.</param>
            public void Add(Type type, string dbType) => Add(type, dbType, null);

            /// <summary>
            /// Adds the specified type to this map.
            /// </summary>
            /// <param name="type">The .NET type to map.</param>
            /// <param name="dbType">The name of the Postgres type to assign to
            /// parameters.</param>
            /// <param name="extractor">A delegate which extracts the value to
            /// be assigned to the parameter.</param>
            public void Add(Type type, string dbType, Func<object, object> extractor)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                if (dbType == null)
                {
                    throw new ArgumentNullException(nameof(dbType));
                }

                _values.Add(type, (dbType, extractor));
            }


            /// <summary>
            /// Adds the specified type to this map.
            /// </summary>
            /// <param name="type">The .NET type to map.</param>
            /// <param name="dbType">The Postgres type to assign to parameters.</param>
            public void Add(Type type, NpgsqlDbType dbType) => Add(type, dbType, null);

            /// <summary>
            /// Adds the specified type to this map.
            /// </summary>
            /// <param name="type">The .NET type to map.</param>
            /// <param name="dbType">The Postgres type to assign to parameters.</param>
            /// <param name="extractor">A delegate which extracts the value to
            /// be assigned to parameters.</param>
            public void Add(Type type, NpgsqlDbType dbType, Func<object, object> extractor)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                _values.Add(type, (dbType, extractor));
            }


            IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
        }

        /// <summary>
        /// Sets the parameter value to be used for the given object.
        /// </summary>
        /// <param name="parameter">The parameter to be set.</param>
        /// <param name="value">The input value.</param>
        public override void SetValue(DbParameter parameter, object value)
        {
            if (value != null && parameter is NpgsqlParameter npgsqlParameter)
            {
                var valueType = value.GetType();
                var rootType = Nullable.GetUnderlyingType(valueType) ?? valueType;

                if (rootType.Namespace == nameof(System))
                {
                    base.SetValue(parameter, value);
                }
                else if (CustomTypes.TryGetMapping(valueType, out object dbType, out Func<object,object> extractor))
                {
                    switch (dbType)
                    {
                        case string typeName:
                            npgsqlParameter.DataTypeName = typeName;
                            npgsqlParameter.Value = extractor == null ? value : extractor(value);
                            break;
                        case NpgsqlDbType typeEnum:
                            npgsqlParameter.NpgsqlDbType = typeEnum;
                            npgsqlParameter.Value = extractor == null ? value : extractor(value);
                            break;
                        default:
                            base.SetValue(parameter, value);
                            break;
                    }
                }
                else
                {
                    npgsqlParameter.NpgsqlDbType = UseJsonb ? NpgsqlDbType.Jsonb : NpgsqlDbType.Json;
                    npgsqlParameter.Value = value;
                }
            }
            else
            {
                base.SetValue(parameter, value);
            }
        }
    }
}
