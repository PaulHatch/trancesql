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
    }
}
