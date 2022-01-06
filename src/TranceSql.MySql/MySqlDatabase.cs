using OpenTracing;
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
        /// <param name="connectionFactory">
        /// A connection factory that returns a MySQL DB connection.
        /// </param>
        public MySqlDatabase(MySqlConnectionFactory connectionFactory)
            : this(connectionFactory, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a MySQL DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public MySqlDatabase(MySqlConnectionFactory connectionFactory, IParameterMapper parameterMapper)
            : this(connectionFactory, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a MySQL DB connection.
        /// </param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public MySqlDatabase(MySqlConnectionFactory connectionFactory, ITracer tracer)
            : this(connectionFactory, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a MySQL database reference.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a MySQL DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public MySqlDatabase(MySqlConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), tracer), new MySqlDialect())
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
        /// <param name="parameterMapper">The parameter mapper.</param>
        public MySqlDatabase(string connectionString, IParameterMapper parameterMapper)
            : this(connectionString, parameterMapper, null)
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
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public MySqlDatabase(string connectionString, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(new MySqlConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), tracer), new MySqlDialect())
        {
        }
    }
}
