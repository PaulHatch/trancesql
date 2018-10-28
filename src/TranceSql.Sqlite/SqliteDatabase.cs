using Microsoft.Data.Sqlite;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using TranceSql.Processing;

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
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public SqliteDatabase(string connectionString, IParameterValueExtractor extractor)
            : this(connectionString, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqliteDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }


        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqliteDatabase(string connectionString, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo(connectionString)), new SqliteDialect())
        {
        }

        private static DbInfo ExtractDbInfo(string connectionString)
        {
            var builder = new SqliteConnectionStringBuilder(connectionString);
            return new DbInfo(builder.DataSource, null, null);
        }

        private static DbConnection GetConnection() => new SqliteConnection();
    }
}
