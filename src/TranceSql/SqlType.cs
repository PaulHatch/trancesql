using System;
using System.Collections.Generic;
using System.Data;

namespace TranceSql
{
    /// <summary>
    /// Represents a SQL type for a <see cref="ColumnDefinition"/>.
    /// </summary>
    public class SqlType : ISqlElement
    {
        /// <summary>
        /// Gets or sets the name of the type. If this value is set, the <see cref="Type"/>
        /// property is ignored.
        /// </summary>
        public string ExplicitTypeName { get; set; }

        /// <summary>
        /// Gets or sets the DbType type. This value will be resolved to the appropriate
        /// type by the SQL dialect used to render the command.
        /// </summary>
        public DbType Type { get; set; }

        /// <summary>
        /// Gets or sets the type parameters, for example in VARCHAR(50), '50' is the type parameter.
        /// </summary>
        public IEnumerable<object> Parameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is allowed to be null.
        /// </summary>
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


        /// <summary>
        /// Initializes a new instance of the <see cref="SqlType" /> class.
        /// </summary>
        /// <param name="typeClass">The type, this value will be resolved to the appropriate
        /// type by the SQL dialect used to render the command.</param>
        /// <param name="allowNull">Indicates whether the type is allowed to be null.</param>
        /// <param name="parameters">The type parameters, for example in VARCHAR(50), '50' is the type parameter.</param>
        public SqlType(DbType typeClass, bool allowNull, IEnumerable<object> parameters)
        {
            Type = typeClass;
            Parameters = parameters;
            AllowNull = allowNull;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlType" /> class.
        /// </summary>
        /// <param name="typeClass">The type, this value will be resolved to the appropriate
        /// type by the SQL dialect used to render the command.</param>
        /// <param name="allowNull">Indicates whether the type is allowed to be null.</param>
        /// <param name="parameters">The type parameters, for example in VARCHAR(50), '50' is the type parameter.</param>
        public SqlType(DbType typeClass, bool allowNull, params object[] parameters)
        {
            Type = typeClass;
            Parameters = parameters;
            AllowNull = allowNull;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlType" /> class.
        /// </summary>
        /// <param name="explicitTypeName">The exact name of the type.</param>
        /// <param name="allowNull">Indicates whether the type is allowed to be null.</param>
        public SqlType(string explicitTypeName, bool allowNull)
        {
            if (string.IsNullOrWhiteSpace(explicitTypeName))
            {
                throw new ArgumentException("Type name must have a value.", nameof(explicitTypeName));
            }

            ExplicitTypeName = explicitTypeName;
            AllowNull = allowNull;
        }

        /// <summary>
        /// Creates a new <see cref="SqlType"/> instance from a .NET type.
        /// </summary>
        /// <typeparam name="T">The type to derive from.</typeparam>
        /// <param name="parameters">
        /// The type parameters, for example in VARCHAR(50), '50' is the type parameter.
        /// </param>
        /// <param name="allowNull">Indicates whether the type is allowed to be null.</param>
        /// <returns>A new <see cref="SqlType"/> instance.</returns>
        public static SqlType From<T>(bool? allowNull = null, params object[] parameters)
            => From(typeof(T), allowNull, parameters);

        /// <summary>
        /// Creates a new <see cref="SqlType" /> instance from a .NET type.
        /// </summary>
        /// <param name="type">The type to derive from.</param>
        /// <param name="allowNull">Indicates whether the type is allowed to be null.</param>
        /// <param name="parameters">The type parameters, for example in VARCHAR(50), '50' is the type parameter.</param>
        /// <returns>
        /// A new <see cref="SqlType" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">type</exception>
        /// <exception cref="ArgumentException">type</exception>
        public static SqlType From(Type type, bool? allowNull = null, params object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var nullableType = Nullable.GetUnderlyingType(type);

            // if no allow null value was specified and a nullable type was used, default to allow null
            if (!allowNull.HasValue && nullableType != null)
            {
                allowNull = true;
            }

            if(_typeMap.ContainsKey(nullableType ?? type))
            {
                return new SqlType(_typeMap[nullableType ?? type], allowNull ?? type.IsClass, parameters);
            }

            throw new ArgumentException($"Error in SqlType.From conversion: Automatic mapping is not supported for the type '{type.FullName}'.", nameof(type));
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (string.IsNullOrEmpty(ExplicitTypeName))
            {
                context.Write(context.Dialect.FormatType(Type, Parameters));
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}