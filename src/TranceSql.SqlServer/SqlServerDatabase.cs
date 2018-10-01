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
    /// Creates command parameters for a Microsoft SQL Server database reference.
    /// </summary>
    public class SqlServerDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlServerDatabase(string connectionString)
            : base(new SqlCommandManager(connectionString, GetConnection, new DefaultValueExtractor()), new SqlServerDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The extractor.</param>
        public SqlServerDatabase(string connectionString, IParameterValueExtractor extractor)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor), new SqlServerDialect())
        {
        }

        private static DbDataAdapter GetAdapter() => new SqlDataAdapter();

        private static DbConnection GetConnection() => new SqlConnection();
    }
}
