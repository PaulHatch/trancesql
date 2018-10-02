using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
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

        public string FormatType(DbType type, int? parameter)
        {
            return parameter.HasValue ? $"{GetType(type)}({parameter.Value})" : GetType(type);
        }

        private string GetType(DbType type)
        {
            var parameter = new SqliteParameter
            {
                DbType = type
            };

            switch (parameter.SqliteType)
            {
                case SqliteType.Integer:
                    return "INTEGER";
                case SqliteType.Real:
                    return "REAL";
                case SqliteType.Text:
                    return "TEXT";
                case SqliteType.Blob:
                    return "BLOB";
                default:
                    throw new InvalidCommandException($"The type {parameter.SqliteType} is not supported");
            }
        }
    }
}
