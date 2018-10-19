﻿using Microsoft.Data.Sqlite;
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
        /// <summary>
        /// Gets the limit keyword behavior used by this dialect.
        /// </summary>
        public LimitBehavior LimitBehavior => LimitBehavior.Limit;

        /// <summary>
        /// Gets the offset behavior and support used by this dialect.
        /// </summary>
        public OffsetBehavior OffsetBehavior => OffsetBehavior.Offset;

        /// <summary>
        /// Formats a date constant.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>
        /// A date constant string.
        /// </returns>
        public string FormatDate(DateTime date) => $"'{date}'";

        /// <summary>
        /// Formats a date constant.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>
        /// A date constant string.
        /// </returns>
        public string FormatDate(DateTimeOffset date) => $"'{date}'";

        /// <summary>
        /// Returns a string with the provider-specific escaping applied
        /// for an identifier like a table or column name.
        /// </summary>
        /// <param name="identifier">The raw identifier name.</param>
        /// <returns>
        /// A correctly formatted string.
        /// </returns>
        public string FormatIdentifier(string identifier) => $"\"{identifier}\"";

        /// <summary>
        /// Escapes and formats a string constant.
        /// </summary>
        /// <param name="value">The string value to format.</param>
        /// <returns>
        /// A formatted string constant.
        /// </returns>
        public string FormatString(string value) => $"'{value.Replace("'", "''")}'";

        /// <summary>
        /// Creates a string representing the specified type.
        /// </summary>
        /// <param name="type">The SQL type class.</param>
        /// <param name="parameters">The type parameters, if any.</param>
        /// <returns>
        /// The name of the parameter type for this dialect.
        /// </returns>
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
            var parameter = new SqliteParameter
            {
                DbType = type
            };

            return parameter.SqliteType.ToString().ToUpper();
        }
    }
}
