using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TranceSql
{
    /// <summary>
    /// Minimal, mostly agnostic dialect which can be used for testing
    /// and debugging.
    /// </summary>
    public class GenericDialect : IDialect
    {
        /// <inheritdoc/>
        public LimitBehavior LimitBehavior { get; set; } = LimitBehavior.Limit;

        /// <inheritdoc/>
        public OffsetBehavior OffsetBehavior { get; set; } = OffsetBehavior.Offset;

        /// <inheritdoc/>
        public OutputType OutputType => OutputType.Returning;

        /// <inheritdoc/>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatIdentifier(string identifier) => identifier;

        /// <inheritdoc/>
        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";

        /// <inheritdoc/>
        public string FormatType(DbType type, IEnumerable<object>? parameters)
        {
            var typeName = type.ToString().ToUpper();
            if (parameters?.Any() == true)
            {
                return $"{typeName}({string.Join(", ", parameters)})";
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
                case Isolation.Snapshot:
                    context.Write("BEGIN TRANSACTION ISOLATION LEVEL SNAPSHOT");
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
    }
}
