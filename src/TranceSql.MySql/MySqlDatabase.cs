using System.Diagnostics;
using TranceSql.Processing;

namespace TranceSql.MySql;

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
    public MySqlDatabase(MySqlConnectionFactory connectionFactory, IParameterMapper? parameterMapper)
        : this(connectionFactory, parameterMapper, null)
    {
    }

    /// <summary>
    /// Creates command parameters for a MySQL database reference.
    /// </summary>
    /// <param name="connectionFactory">
    /// A connection factory that returns a MySQL DB connection.
    /// </param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public MySqlDatabase(MySqlConnectionFactory connectionFactory, ActivitySource? activitySource)
        : this(connectionFactory, null, activitySource)
    {
    }

    /// <summary>
    /// Creates command parameters for a MySQL database reference.
    /// </summary>
    /// <param name="connectionFactory">
    /// A connection factory that returns a MySQL DB connection.
    /// </param>
    /// <param name="parameterMapper">The parameter mapper.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public MySqlDatabase(MySqlConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ActivitySource? activitySource)
        : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), activitySource), new MySqlDialect())
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
    public MySqlDatabase(string connectionString, IParameterMapper? parameterMapper)
        : this(connectionString, parameterMapper, null)
    {
    }

    /// <summary>
    /// Creates command parameters for a MySQL database reference.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public MySqlDatabase(string connectionString, ActivitySource? activitySource)
        : this(connectionString, null, activitySource)
    {
    }


    /// <summary>
    /// Creates command parameters for a MySQL database reference.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="parameterMapper">The parameter mapper.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public MySqlDatabase(string connectionString, IParameterMapper? parameterMapper, ActivitySource? activitySource)
        : base(new SqlCommandManager(new MySqlConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), activitySource), new MySqlDialect())
    {
    }
}