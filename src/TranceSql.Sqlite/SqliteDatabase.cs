using System.Diagnostics;
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
        public SqliteDatabase(string connectionString, IParameterMapper? parameterMapper)
            : this(connectionString, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="activitySource">
        /// An activity source that can be used to create activities for database operations.
        /// </param>
        public SqliteDatabase(string connectionString, ActivitySource? activitySource)
            : this(connectionString, null, activitySource)
        {
        }


        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="activitySource">
        /// An activity source that can be used to create activities for database operations.
        /// </param>
        public SqliteDatabase(string connectionString, IParameterMapper? parameterMapper, ActivitySource? activitySource)
            : base(new SqlCommandManager(new SqliteConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), activitySource), new SqliteDialect())
        {
        }
        
        /// <summary>
        /// Creates command parameters for a SQLite connection.
        /// </summary>
        /// <param name="connectionFactory">
        /// A connection factory that returns a Sqlite DB connection.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="activitySource">
        /// An activity source that can be used to create activities for database operations.
        /// </param>
        public SqliteDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ActivitySource? activitySource)
            : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), activitySource), new SqliteDialect())
        {
        }
    }
}
