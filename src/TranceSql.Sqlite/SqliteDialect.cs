using System;
using System.Collections.Generic;
using System.Text;
using TranceSql.Language;

namespace TranceSql.Sqlite
{
    public class SqliteDialect : IDialect
    {
        public LimitBehavior LimitBehavior => LimitBehavior.Limit;

        public OffsetBehavior OffsetBehavior => OffsetBehavior.Offset;

        public string FormatDate(DateTime date) => $"'{date}'";

        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";

        public string FormatType(SqlTypeClass typeClass, int? parameter)
        {
            return parameter.HasValue ? $"{GetType(typeClass)}({parameter.Value})" : GetType(typeClass);
        }

        private string GetType(SqlTypeClass typeClass)
        {
            switch (typeClass)
            {
                case SqlTypeClass.VarChar:
                case SqlTypeClass.Text:
                case SqlTypeClass.Char:
                    return "TEXT";
                case SqlTypeClass.Boolean:
                case SqlTypeClass.Integer:
                case SqlTypeClass.UnsignedInteger:
                    return "INTEGER";
                case SqlTypeClass.Float:
                case SqlTypeClass.Fixed:
                    return "REAL";
                case SqlTypeClass.Binary:
                    return "BLOB";
                default:
                    throw new NotSupportedException($"SQLite does not support the {typeClass} type.");
            }
        }
    }
}
