using System;
using TranceSql.Language;

namespace TranceSql.Postgres
{
    public class PostgresDialect : IDialect
    {
        public LimitBehavior LimitBehavior => LimitBehavior.LimitAndOffset;

        public string FormatDate(DateTime date) => $"'{date}'";
        
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        public string FormatString(string value) => $"'{value.Replace("'","''")}'";

    }
}
