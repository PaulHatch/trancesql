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
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        public OracleDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl)
            : this(asyncConnectionStringFactory, ttl, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public OracleDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor)
            : this(asyncConnectionStringFactory, ttl, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public OracleDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, ITracer tracer)
            : this(asyncConnectionStringFactory, ttl, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public OracleDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(asyncConnectionStringFactory, ttl, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new OracleDialect())
        {
        }


        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        public OracleDatabase(Func<string> connectionStringFactory, TimeSpan ttl)
            : this(connectionStringFactory, ttl, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public OracleDatabase(Func<string> connectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor)
            : this(connectionStringFactory, ttl, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public OracleDatabase(Func<string> connectionStringFactory, TimeSpan ttl, ITracer tracer)
            : this(connectionStringFactory, ttl, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Oracle database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public OracleDatabase(Func<string> connectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionStringFactory, ttl, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new OracleDialect())
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
