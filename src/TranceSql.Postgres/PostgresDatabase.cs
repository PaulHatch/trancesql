using System;
using System.Collections.Generic;
using System.Data.Common;
using Npgsql;
using System.Text;
using TranceSql.Language;
using TranceSql.Processing;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Creates command parameters for a PostgreSQL database reference.
    /// </summary>
    public class PostgresDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgresDatabase(string connectionString)
            : base(new SqlCommandManager(connectionString, GetConnection, new DefaultValueExtractor()), new PostgresDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor), new PostgresDialect())
        {
        }
        
        private static DbConnection GetConnection() => new NpgsqlConnection();
    }
}
