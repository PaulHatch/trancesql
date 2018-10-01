using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using TranceSql.Processing;
using Microsoft.Data.Sqlite;

namespace TranceSql.Sqlite
{
    /// <summary>
    /// Creates command parameters for a SQLite database reference.
    /// </summary>
    public class SqliteDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqliteDatabase(string connectionString)
            : base(new SqlCommandManager(connectionString, GetConnection, new DefaultValueExtractor()), new SqliteDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public SqliteDatabase(string connectionString, IParameterValueExtractor extractor)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor), new SqliteDialect())
        {
        }

        private static DbConnection GetConnection() => new SqliteConnection();
    }
}
