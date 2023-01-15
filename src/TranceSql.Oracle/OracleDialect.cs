using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace TranceSql.Oracle
{
    /// <summary>
    /// Dialect definition for Oracle.
    /// </summary>
    public class OracleDialect : IDialect
    {
        /// <inheritdoc/>
        public LimitBehavior LimitBehavior => LimitBehavior.RowNumAutomatic;

        /// <inheritdoc/>
        public OffsetBehavior OffsetBehavior => OffsetBehavior.None;

        /// <inheritdoc/>
        public OutputType OutputType => OutputType.Returning;

        /// <inheritdoc/>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        /// <inheritdoc/>
        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";

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
                    context.Write("BEGIN TRANSACTION");
                    break;
                case Isolation.ReadUncommitted:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
                    context.Write("BEGIN TRANSACTION");
                    break;
                case Isolation.ReadCommitted:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");
                    context.Write("BEGIN TRANSACTION");
                    break;
                case Isolation.RepeatableRead:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;");
                    context.Write("BEGIN TRANSACTION");
                    break;
                case Isolation.Snapshot:
                    context.Write("BEGIN TRANSACTION WITH CONSISTENT SNAPSHOT;");
                    break;
                case Isolation.Serializable:
                    context.WriteLine("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;");
                    context.Write("BEGIN TRANSACTION");
                    break;
                default:
                    throw new InvalidCommandException($"Unsupported isolation level '{beginTransaction.Isolation}'");
            }

            context.Write(';');
        }
        
        /// <summary>
        /// Creates a string representing the specified type.
        /// </summary>
        /// <param name="type">The SQL type class.</param>
        /// <param name="parameters">The type parameters, if any.</param>
        /// <returns>
        /// The name of the parameter type for this dialect.
        /// </returns>
        public string FormatType(DbType type, IEnumerable<object>? parameters)
        {
            var typeName = GetType(type);
            if (parameters?.Any() == true)
            {
                return $"{typeName}({string.Join(", ", parameters)})";
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
