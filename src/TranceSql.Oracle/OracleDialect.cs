using System;
using System.Collections.Generic;
using System.Data;
using TranceSql.Language;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace TranceSql.Oracle
{
    public class OracleDialect : IDialect
    {
        public LimitBehavior LimitBehavior => LimitBehavior.RowNumAutomatic;

        public OffsetBehavior OffsetBehavior => OffsetBehavior.None;

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
            var parameter = new OracleParameter
            {
                DbType = type
            };

            return parameter.OracleDbType.ToString().ToUpper();
        }
    }
}
