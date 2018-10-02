using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using TranceSql.Processing;

namespace TranceSql.MySql
{
    /// <summary>
    /// Creates command parameters for a MySql database reference.
    /// </summary>
    public class PostgresDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a MySql database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgresDatabase(string connectionString)
            : base(new SqlCommandManager(connectionString, GetConnection, new DefaultValueExtractor()), new MySqlDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a MySql database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor), new MySqlDialect())
        {
        }

        private static DbConnection GetConnection() => new MySqlConnection();
    }
}
