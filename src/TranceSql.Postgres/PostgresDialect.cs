using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Linq;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Dialect definition for Postgres.
    /// </summary>
    public class PostgresDialect : IDialect
    {
        /// <inheritdoc/>
        public LimitBehavior LimitBehavior => LimitBehavior.Limit;

        /// <inheritdoc/>
        public OffsetBehavior OffsetBehavior => OffsetBehavior.Offset;

        /// <inheritdoc/>
        public OutputType OutputType => OutputType.Returning;

        /// <inheritdoc/>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        /// <inheritdoc/>
        public string FormatString(string value) => $"'{value.Replace("'","''")}'";

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
            switch (beginTransaction.Isolation)
            {
                case null:
                    context.Write("BEGIN TRANSACTION");
                    break;
                case Isolation.ReadUncommitted:
                    context.Write("BEGIN TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                    break;
                case Isolation.ReadCommitted:
                    context.Write("BEGIN TRANSACTION ISOLATION LEVEL READ COMMITTED");
                    break;
                case Isolation.RepeatableRead:
                    context.Write("BEGIN TRANSACTION ISOLATION LEVEL REPEATABLE READ");
                    break;
                case Isolation.Serializable:
                    context.Write("BEGIN TRANSACTION ISOLATION LEVEL SERIALIZABLE");
                    break;
                default:
                    throw new InvalidCommandException($"Unsupported isolation level '{beginTransaction.Isolation}'");
            }

            if (beginTransaction.ReadOnly.HasValue)
            {
                if (beginTransaction.Isolation != null)
                {
                    context.Write(',');
                }
                context.Write(beginTransaction.ReadOnly.Value ? " READ ONLY" : " READ WRITE");
            }
            context.Write(';');
        }

        private string GetType(DbType type)
        {
            var parameter = new NpgsqlParameter
            {
                DbType = type
            };

            return parameter.NpgsqlDbType.ToString().ToUpper();
        }
    }
}
