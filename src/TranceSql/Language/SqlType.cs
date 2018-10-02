using System;
using System.Collections.Generic;
using System.Data;

namespace TranceSql.Language
{
    public class SqlType : ISqlElement
    {
        public string ExplicitTypeName { get; set; }
        public DbType Type { get; set; }
        public int? Parameter { get; set; }
        public bool AllowNull { get; set; }

        static readonly IReadOnlyDictionary<Type, DbType> _typeMap = new Dictionary<Type, DbType>
        {
            { typeof(byte), DbType.Byte },
            { typeof(sbyte), DbType.SByte },
            { typeof(short), DbType.Int16 },
            { typeof(ushort), DbType.UInt16 },
            { typeof(int), DbType.Int32 },
            { typeof(uint), DbType.UInt32 },
            { typeof(long), DbType.Int64 },
            { typeof(ulong), DbType.UInt64 },
            { typeof(float), DbType.Single },
            { typeof(double), DbType.Double },
            { typeof(decimal), DbType.Decimal },
            { typeof(bool), DbType.Boolean },
            { typeof(string), DbType.String },
            { typeof(char), DbType.StringFixedLength },
            { typeof(Guid), DbType.Guid },
            { typeof(DateTime), DbType.DateTime },
            { typeof(DateTimeOffset), DbType.DateTimeOffset },
            { typeof(byte[]), DbType.Binary },
            { typeof(byte?), DbType.Byte },
            { typeof(sbyte?), DbType.SByte },
            { typeof(short?), DbType.Int16 },
            { typeof(ushort?), DbType.UInt16 },
        };


        public SqlType(DbType typeClass, int? parameter, bool allowNull)
        {
            Type = typeClass;
            Parameter = parameter;
            AllowNull = allowNull;
        }

        public SqlType(string explicitTypeName, bool allowNull)
        {
            if (string.IsNullOrWhiteSpace(explicitTypeName))
            {
                throw new ArgumentException("Type name must have a value.", nameof(explicitTypeName));
            }

            ExplicitTypeName = explicitTypeName;
            AllowNull = allowNull;
        }

        public static SqlType From<T>(int? parameter = null, bool? allowNull = null)
        {
            var type = typeof(T);
            var nullableType = Nullable.GetUnderlyingType(type);

            // if no allow null value was specified and a nullable type was used, default to allow null
            if (!allowNull.HasValue && nullableType != null)
            {
                allowNull = true;
            }

            if(_typeMap.ContainsKey(nullableType ?? type))
            {
                return new SqlType(_typeMap[nullableType ?? type], parameter, allowNull ?? true);
            }


            throw new ArgumentException($"Error in  SqlType.From<{type.Name}>() Automatic mapping is not supported for the type '{type.FullName}'.", nameof(T));
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (string.IsNullOrEmpty(ExplicitTypeName))
            {
                context.Write(context.Dialect.FormatType(Type, Parameter));
            }
            else
            {
                context.Write(ExplicitTypeName);
            }
            
            if (AllowNull)
            {
                context.Write(" NULL");
            }
            else
            {
                context.Write(" NOT NULL");
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}