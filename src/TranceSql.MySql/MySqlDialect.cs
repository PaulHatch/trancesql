using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TranceSql.MySql
{
    /// <summary>
    /// Dialect definition for MySQL.
    /// </summary>
    public class MySqlDialect : IDialect
    {
        /// <inheritdoc/>
        public LimitBehavior LimitBehavior => LimitBehavior.RowNumAutomatic;

        /// <inheritdoc/>
        public OffsetBehavior OffsetBehavior => OffsetBehavior.None;

        /// <inheritdoc/>
        public OutputType OutputType => OutputType.None;

        /// <inheritdoc/>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <inheritdoc/>
        public string FormatIdentifier(string identifier) => $"`{identifier}`";

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
                switch (type)
                {
                    case DbType.AnsiString:
                        return $"VARCHAR(1000)";
                    case DbType.String:
                        return $"VARCHAR(1000) CHARACTER SET utf8";
                    default:
                        return typeName;
                }
            }
        }

        /// <inheritdoc/>
        public void Render(RenderContext context, BeginTransaction beginTransaction)
        {   
            switch (beginTransaction.Isolation)
            {
                case null:
                    context.Write("START TRANSACTION"); 
                    break;
                case Isolation.Snapshot:
                    context.Write("START TRANSACTION WITH CONSISTENT SNAPSHOT");
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
            var parameter = new MySqlParameter
            {
                DbType = type
            };

            switch (parameter.MySqlDbType)
            {
                case MySqlDbType.Bool: return "BIT";
                case MySqlDbType.Decimal: return "DECIMAL";
                case MySqlDbType.Byte: return "TINYINT";
                case MySqlDbType.Int16: return "SMALLINT";
                case MySqlDbType.Int24: return "MEDIUMINT";
                case MySqlDbType.Int32: return "INT";
                case MySqlDbType.Int64: return "BIGINT";
                case MySqlDbType.Float: return "FLOAT";
                case MySqlDbType.Double: return "DOUBLE";
                default: return parameter.MySqlDbType.ToString().ToUpper();
            }
        }
    }
}
