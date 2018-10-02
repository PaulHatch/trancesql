using System;
using System.Data;
using TranceSql.Language;

namespace TranceSql.SqlServer
{
    public class SqlServerDialect : IDialect
    {
        public LimitBehavior LimitBehavior => LimitBehavior.Top;

        public OffsetBehavior OffsetBehavior => OffsetBehavior.None;

        public string FormatDate(DateTime date) => $"'{date}'";
        
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => $"[{identifier}]";

        public string FormatString(string value) => $"N'{value.Replace("'","''")}'";

        public string FormatType(DbType type, int? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
