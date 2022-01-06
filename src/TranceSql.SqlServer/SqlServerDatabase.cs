using OpenTracing;
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
        /// <param name="connectionFactory">
        /// A connection factory that returns a SQL Server DB connection.
        /// </param>
        public SqlServerDatabase(IConnectionFactory connectionFactory)
            : this(connectionFactory, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a SQL Server DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public SqlServerDatabase(IConnectionFactory connectionFactory, IParameterMapper parameterMapper)
            : this(connectionFactory, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a SQL Server DB connection.
        /// </param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqlServerDatabase(IConnectionFactory connectionFactory, ITracer tracer)
            : this(connectionFactory, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Microsoft SQL Server database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a SQL Server DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public SqlServerDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), tracer), new SqlServerDialect())
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
        public SqlServerDatabase(string connectionString, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(new SqlServerConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), tracer), new SqlServerDialect())
        {
        }
    }
}
