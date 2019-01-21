using MySql.Data.MySqlClient;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql.MySql
{
    /// <summary>
    /// Creates command parameters for a MySql database reference.
    /// </summary>
    public class MySqlDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        public MySqlDatabase(MySqlRollingCredentials rollingCredentials)
            : this(rollingCredentials, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="extractor">The parameter value extractor.</param>
        public MySqlDatabase(MySqlRollingCredentials rollingCredentials, IParameterValueExtractor extractor)
            : this(rollingCredentials, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public MySqlDatabase(MySqlRollingCredentials rollingCredentials, ITracer tracer)
            : this(rollingCredentials, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public MySqlDatabase(MySqlRollingCredentials rollingCredentials, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(rollingCredentials, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new MySqlDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySqlDatabase(string connectionString)
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public MySqlDatabase(string connectionString, IParameterValueExtractor extractor)
            : this(connectionString, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public MySqlDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }


        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public MySqlDatabase(string connectionString, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo(connectionString)), new MySqlDialect())
        {
        }

        private static DbInfo ExtractDbInfo(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            return new DbInfo(builder.Server, builder.Database, builder.UserID);
        }

        private static DbConnection GetConnection() => new MySqlConnection();
    }
}
