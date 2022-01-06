using OpenTracing;
using TranceSql.Processing;

namespace TranceSql.Sqlite
{
    /// <summary>
    /// Creates command parameters for a SQLite database reference.
    /// </summary>
    public class SqliteDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqliteDatabase(string connectionString)
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public SqliteDatabase(string connectionString, IParameterMapper parameterMapper)
            : this(connectionString, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqliteDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }


        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqliteDatabase(string connectionString, IParameterMapper? parameterMapper, ITracer? tracer)
            : base(new SqlCommandManager(new SqliteConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), tracer), new SqliteDialect())
        {
        }
        
        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a Sqlite DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public SqliteDatabase(IConnectionFactory connectionFactory, IParameterMapper parameterMapper, ITracer tracer)
            : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), tracer), new SqliteDialect())
        {
        }
    }
}
