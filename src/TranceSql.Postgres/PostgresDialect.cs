using System;
using TranceSql.Language;

namespace TranceSql.Postgres
{
    public class PostgresDialect : IDialect
    {
        public LimitBehavior LimitBehavior => LimitBehavior.Limit;

        public OffsetBehavior OffsetBehavior => OffsetBehavior.None;

        public string FormatDate(DateTime date) => $"'{date}'";
        
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        public string FormatString(string value) => $"'{value.Replace("'","''")}'";

        public string FormatType(SqlTypeClass typeClass, int? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
