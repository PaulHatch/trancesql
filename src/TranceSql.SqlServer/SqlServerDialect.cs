using System;
using TranceSql.Language;

namespace TranceSql.SqlServer
{
    public class SqlServerDialect : IDialect
    {
        public LimitBehavior LimitBehavior => LimitBehavior.Top;

        public string FormatDate(DateTime date) => $"'{date}'";
        
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => $"[{identifier}]";

        public string FormatString(string value) => $"N'{value.Replace("'","''")}'";
    }
}
