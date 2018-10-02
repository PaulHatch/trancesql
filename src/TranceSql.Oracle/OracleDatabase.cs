using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using TranceSql.Processing;

namespace TranceSql.Oracle
{
    /// <summary>
    /// Creates command parameters for a Oracle database reference.
    /// </summary>
    public class PostgresDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgresDatabase(string connectionString)
            : base(new SqlCommandManager(connectionString, GetConnection, new DefaultValueExtractor()), new OracleDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor), new OracleDialect())
        {
        }

        private static DbConnection GetConnection() => new OracleConnection();
    }
}
