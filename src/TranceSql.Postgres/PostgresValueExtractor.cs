using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Configurable implementation of <see cref="IParameterValueExtractor"/>
    /// for handling Postgres parameters. When providing values, types not in the
    /// "System" namespace will be specified as JSON or JSONB (as configured).
    /// </summary>
    public class PostgresValueExtractor : DefaultValueExtractor
    {
        /// <summary>
        /// Gets a value indicating whether to set the parameter type to JSONB
        /// when automatically assigning the parameter type for unknown values.
        /// If true JSONB will be used, if false, JSON will be used instead.
        /// </summary>
        public bool UseJsonb { get; } = true;

        /// <summary>
        /// Gets the custom type map. When the type specified in the map is
        /// provided to a command, the corresponding type name or DB type will
        /// be used as the Postgres type name.
        /// </summary>
        public IDictionary<Type, Any<string, NpgsqlDbType>> CustomTypes { get; }
            = new Dictionary<Type, Any<string, NpgsqlDbType>>();
        
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
                else if(CustomTypes.ContainsKey(valueType))
                {
                    switch (CustomTypes[valueType].Value)
                    {
                        case string typeName:
                            npgsqlParameter.DataTypeName = typeName;
                            npgsqlParameter.Value = value;
                            break;
                        case NpgsqlDbType dbType:
                            npgsqlParameter.NpgsqlDbType = dbType;
                            npgsqlParameter.Value = value;
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
