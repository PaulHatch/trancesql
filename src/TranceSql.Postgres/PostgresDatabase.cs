using System;
using System.Collections.Generic;
using System.Data.Common;
using Npgsql;
using System.Text;
using TranceSql.Processing;
using OpenTracing;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Creates command parameters for a PostgreSQL database reference.
    /// </summary>
    public class PostgresDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgresDatabase(string connectionString)
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor)
            : this(connectionString, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo(connectionString)), new PostgresDialect())
        {
        }

        private static DbInfo ExtractDbInfo(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            return new DbInfo(builder.Host, builder.Database, builder.Username);
        }

        private static DbConnection GetConnection() => new NpgsqlConnection();
    }
}
