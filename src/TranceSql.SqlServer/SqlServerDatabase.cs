using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        public SqlServerDatabase(SqlServerRollingCredentials rollingCredentials)
            : this(rollingCredentials, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public SqlServerDatabase(SqlServerRollingCredentials rollingCredentials, IParameterMapper parameterMapper)
            : this(rollingCredentials, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqlServerDatabase(SqlServerRollingCredentials rollingCredentials, ITracer tracer)
            : this(rollingCredentials, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public SqlServerDatabase(SqlServerRollingCredentials rollingCredentials, IParameterMapper parameterMapper, ITracer tracer)
            : base(new SqlCommandManager(rollingCredentials, GetConnection, parameterMapper ?? new DefaultParameterMapper(), tracer, ExtractDbInfo), new SqlServerDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlServerDatabase(string connectionString)
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public SqlServerDatabase(string connectionString, IParameterMapper parameterMapper)
            : this(connectionString, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqlServerDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqlServerDatabase(string connectionString, IParameterMapper parameterMapper, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, parameterMapper ?? new DefaultParameterMapper(), tracer, ExtractDbInfo(connectionString)), new SqlServerDialect())
        {
        }


        private static DbInfo ExtractDbInfo(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return new DbInfo(builder.DataSource, builder.InitialCatalog, builder.UserID);
        }

        private static DbConnection GetConnection() => new SqlConnection();
    }
}
