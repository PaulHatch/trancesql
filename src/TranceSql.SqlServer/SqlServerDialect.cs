using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
            var parameter = new SqlParameter
            {
                DbType = type
            };

            return parameter.SqlDbType.ToString().ToUpper();
        }
    }
}
