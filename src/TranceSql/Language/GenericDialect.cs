using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Minimal, mostly agnostic dialect which can be used for testing
    /// and debugging.
    /// </summary>
    public class GenericDialect : IDialect
    {
        public LimitBehavior LimitBehavior { get; set; } = LimitBehavior.Limit;

        public OffsetBehavior OffsetBehavior { get; set; } = OffsetBehavior.Offset;

        public string FormatDate(DateTime date) => $"'{date}'";

        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => identifier;

        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";

        public string FormatType(DbType type, IEnumerable<object> parameters)
        {
            var typeName = type.ToString().ToUpper();
            if (parameters?.Any() == true)
            {
                return $"{type}({String.Join(", ", parameters)})";
            }
            else
            {
                return typeName;
            }
        }
    }
}
