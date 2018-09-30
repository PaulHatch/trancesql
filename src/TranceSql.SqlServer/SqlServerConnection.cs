using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using TranceSql.Language;
using TranceSql.Processing;

namespace TranceSql.SqlServer
{
    /// <summary>
    /// Creates command parameters for a Microsoft SQL Server connection.
    /// </summary>
    public class SqlServerConnection : Connection
    {
        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlServerConnection(string connectionString)
            : base(new SqlCommandManager(connectionString, GetConnection, GetAdapter, new DefaultValueExtractor()), new SqlServerDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public SqlServerConnection(string connectionString, IParameterValueExtractor extractor)
            : base(new SqlCommandManager(connectionString, GetConnection, GetAdapter, extractor), new SqlServerDialect())
        {
        }

        private static DbDataAdapter GetAdapter() => new SqlDataAdapter();

        private static DbConnection GetConnection() => new SqlConnection();
    }
}
