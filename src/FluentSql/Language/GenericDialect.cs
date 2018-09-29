using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Minimal, mostly agnostic dialect which can be used for testing
    /// and debugging.
    /// </summary>
    public class GenericDialect : IDialect
    {
        public LimitBehavior LimitBehavior { get; set; } = LimitBehavior.LimitAndOffset;

        public string FormatDate(DateTime date) => $"'{date}'";

        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        public string FormatIdentifier(string identifier) => identifier;

        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";
    }
}
