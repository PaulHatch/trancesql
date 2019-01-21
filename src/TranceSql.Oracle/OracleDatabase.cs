using OpenTracing;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql.Oracle
{
    /// <summary>
    /// Creates command parameters for a Oracle database reference.
    /// </summary>
    public class OracleDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        public OracleDatabase(OracleRollingCredentials rollingCredentials)
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
        /// <param name="extractor">The parameter value extractor.</param>
        public OracleDatabase(OracleRollingCredentials rollingCredentials, IParameterValueExtractor extractor)
            : this(rollingCredentials, extractor, null)
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
        public OracleDatabase(OracleRollingCredentials rollingCredentials, ITracer tracer)
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
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public OracleDatabase(OracleRollingCredentials rollingCredentials, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(rollingCredentials, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new OracleDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OracleDatabase(string connectionString)
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public OracleDatabase(string connectionString, IParameterValueExtractor extractor)
            : this(connectionString, extractor, null)
        {
        }


        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public OracleDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }


        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public OracleDatabase(string connectionString, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo(connectionString)), new OracleDialect())
        {
        }

        private static DbInfo ExtractDbInfo(string connectionString)
        {
            var builder = new OracleConnectionStringBuilder(connectionString);
            return new DbInfo(builder.DataSource, null, null);
        }

        private static DbConnection GetConnection() => new OracleConnection();
    }
}
