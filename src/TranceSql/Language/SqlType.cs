using System;

namespace TranceSql.Language
{
    public enum SqlTypeClass
    {
        Text,
        Boolean,
        Integer,
        UnsignedInteger,
        Float,
        Fixed,
        Binary,
        VarChar,
        Char,
        Date,
        Time,
        DateTime,
        TimeSpan
    }

    public class SqlType : ISqlElement
    {
        public string ExplicitTypeName { get; set; }
        public SqlTypeClass TypeClass { get; set; }
        public int? Parameter { get; set; }
        public bool AllowNull { get; set; }

        public SqlType(SqlTypeClass typeClass, int? parameter, bool allowNull)
        {
            TypeClass = typeClass;
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

        public static SqlType From<T>(int? parameter = null, bool? allowNull = true)
        {
            var type = typeof(T);

            // if no allow null value was specified and a nullable type was used, default to allow null
            if(!allowNull.HasValue && Nullable.GetUnderlyingType(type) != null)
            {
                allowNull = true;
            }

            if (type == typeof(string)) { return new SqlType(SqlTypeClass.Text, parameter, allowNull ?? false); }
            if (type == typeof(char)) { return new SqlType(SqlTypeClass.Char, parameter ?? 1, allowNull ?? false); }
            if (type == typeof(char[])) { return new SqlType(SqlTypeClass.Char, parameter ?? 1, allowNull ?? false); }
            if (type == typeof(byte)) { return new SqlType(SqlTypeClass.Integer, parameter ?? 1, allowNull ?? false); }
            if (type == typeof(short)) { return new SqlType(SqlTypeClass.Integer, parameter ?? 2, allowNull ?? false); }
            if (type == typeof(int)) { return new SqlType(SqlTypeClass.Integer, parameter ?? 4, allowNull ?? false); }
            if (type == typeof(long)) { return new SqlType(SqlTypeClass.Integer, parameter ?? 8, allowNull ?? false); }
            if (type == typeof(float)) { return new SqlType(SqlTypeClass.Integer, parameter ?? 4, allowNull ?? false); }
            if (type == typeof(double)) { return new SqlType(SqlTypeClass.Integer, parameter ?? 8, allowNull ?? false); }
            if (type == typeof(DateTime)) { return new SqlType(SqlTypeClass.DateTime, parameter, allowNull ?? false); }
            if (type == typeof(DateTimeOffset)) { return new SqlType(SqlTypeClass.DateTime, parameter, allowNull ?? false); }
            if (type == typeof(TimeSpan)) { return new SqlType(SqlTypeClass.TimeSpan, parameter, allowNull ?? false); }
            if (type == typeof(byte[])) { return new SqlType(SqlTypeClass.Binary, parameter, allowNull ?? false); }

            if (type == typeof(ushort)) { return new SqlType(SqlTypeClass.UnsignedInteger, parameter ?? 2, allowNull ?? false); }
            if (type == typeof(uint)) { return new SqlType(SqlTypeClass.UnsignedInteger, parameter ?? 4, allowNull ?? false); }
            if (type == typeof(ulong)) { return new SqlType(SqlTypeClass.UnsignedInteger, parameter ?? 8, allowNull ?? false); }
            
            throw new ArgumentException($"Error in  SqlType.From<{type.Name}>() Automatic mapping is not supported for the type '{type.FullName}'.", nameof(T));
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (string.IsNullOrEmpty(ExplicitTypeName))
            {
                context.Write(context.Dialect.FormatType(TypeClass, Parameter));
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