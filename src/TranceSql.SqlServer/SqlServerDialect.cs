using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TranceSql.SqlServer
{
    /// <summary>
    /// Dialect definition for MS SQL Server.
    /// </summary>
    public class SqlServerDialect : IDialect
    {
        /// <inheritdoc/>
        public LimitBehavior LimitBehavior => LimitBehavior.Top;

        /// <inheritdoc/>
        public OffsetBehavior OffsetBehavior => OffsetBehavior.None;

        /// <inheritdoc/>
        public OutputType OutputType => OutputType.Output;

        /// <inheritdoc/>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatIdentifier(string identifier) => $"[{identifier}]";

        /// <inheritdoc/>
        public string FormatString(string value) => $"N'{value.Replace("'", "''")}'";

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
                switch (type)
                {
                    case DbType.AnsiString:
                    case DbType.String:
                        return $"{typeName}(MAX)";
                    default:
                        return typeName;
                }
            }
        }

        /// <inheritdoc/>
        public void Render(RenderContext context, BeginTransaction beginTransaction)
        {
            if (beginTransaction.ReadOnly.HasValue)
            {
                throw new InvalidCommandException($"BeginTransaction.ReadOnly is not supported by this SQL dialect");
            }

            switch (beginTransaction.Isolation)
            {
                case null:
                    break;
                case Isolation.ReadUncommitted:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
                    break;
                case Isolation.ReadCommitted:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");
                    break;
                case Isolation.RepeatableRead:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;");
                    break;
                case Isolation.Snapshot:
                    context.Write("SET TRANSACTION ISOLATION LEVEL SNAPSHOT;");
                    break;
                case Isolation.Serializable:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;");
                    break;
                default:
                    throw new InvalidCommandException($"Unsupported isolation level '{beginTransaction.Isolation}'");
            }

            context.Write("BEGIN TRANSACTION;");
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
