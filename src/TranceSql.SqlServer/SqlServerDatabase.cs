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
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        public SqlServerDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl)
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
        public SqlServerDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor)
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
        public SqlServerDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, ITracer tracer)
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
        public SqlServerDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(asyncConnectionStringFactory, ttl, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new SqlServerDialect())
        {
        }


        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        public SqlServerDatabase(Func<string> connectionStringFactory, TimeSpan ttl)
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
        public SqlServerDatabase(Func<string> connectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor)
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
        public SqlServerDatabase(Func<string> connectionStringFactory, TimeSpan ttl, ITracer tracer)
            : this(connectionStringFactory, ttl, null, tracer)
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
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public SqlServerDatabase(Func<string> connectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionStringFactory, ttl, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new SqlServerDialect())
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
        /// <param name="extractor">The parameter value extractor.</param>
        public SqlServerDatabase(string connectionString, IParameterValueExtractor extractor)
            : this(connectionString, extractor, null)
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
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqlServerDatabase(string connectionString, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo(connectionString)), new SqlServerDialect())
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
