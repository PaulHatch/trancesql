using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public string FormatType(DbType type, IEnumerable<object> parameters)
        {
            var typeName = GetType(type);
            if (parameters?.Any() == true)
            {
                return $"{type}({String.Join(", ", parameters)})";
            }
            else
            {
                return typeName;
            }
        }

        private string GetType(DbType type)
        {
            var parameter = new SqliteParameter
            {
                DbType = type
            };

            return parameter.SqliteType.ToString().ToUpper();
        }
    }
}
