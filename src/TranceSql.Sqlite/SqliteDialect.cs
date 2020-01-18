using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TranceSql.Sqlite
{
    /// <summary>
    /// Dialect definition for SQLite.
    /// </summary>
    public class SqliteDialect : IDialect
    {
        /// <inheritdoc/>
        public LimitBehavior LimitBehavior => LimitBehavior.Limit;

        /// <inheritdoc/>
        public OffsetBehavior OffsetBehavior => OffsetBehavior.Offset;

        /// <inheritdoc/>
        public OutputType OutputType => OutputType.None;

        /// <inheritdoc/>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        /// <inheritdoc/>
        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";

        /// <inheritdoc/>
        public string FormatType(DbType type, IEnumerable<object> parameters)
        {
            var typeName = GetType(type);
            if (parameters?.Any() == true)
            {
                return $"{typeName}({String.Join(", ", parameters)})";
            }
            else
            {
                return typeName;
            }
        }

        /// <inheritdoc/>
        public void Render(RenderContext context, BeginTransaction beginTransaction)
        {
            if (beginTransaction.Isolation != null)
            {
                throw new InvalidCommandException($"BeginTransaction.Isolation is not supported by this SQL dialect");
            }

            if (beginTransaction.ReadOnly.HasValue)
            {
                throw new InvalidCommandException($"BeginTransaction.ReadOnly is not supported by this SQL dialect");
            }

            context.Write("BEGIN TRANSACTION;");
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
