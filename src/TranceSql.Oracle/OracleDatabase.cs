using OpenTracing;
using TranceSql.Processing;

namespace TranceSql.Oracle
{
    /// <summary>
    /// Creates command parameters for a Oracle database reference.
    /// </summary>
    public class OracleDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for an Oracle database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns an Oracle DB connection.
        /// </param>
        public OracleDatabase(IConnectionFactory connectionFactory)
            : this(connectionFactory, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for an Oracle database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns an Oracle DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public OracleDatabase(IConnectionFactory connectionFactory, IParameterMapper parameterMapper)
            : this(connectionFactory, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for an Oracle database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns an Oracle DB connection.
        /// </param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public OracleDatabase(IConnectionFactory connectionFactory, ITracer tracer)
            : this(connectionFactory, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for an Oracle database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns an Oracle DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public OracleDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), tracer), new OracleDialect())
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
        /// <param name="parameterMapper">The parameter mapper.</param>
        public OracleDatabase(string connectionString, IParameterMapper parameterMapper)
            : this(connectionString, parameterMapper, null)
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
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public OracleDatabase(string connectionString, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(new OracleConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), tracer), new OracleDialect())
        {
        }
    }
}
