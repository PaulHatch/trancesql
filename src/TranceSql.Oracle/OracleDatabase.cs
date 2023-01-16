using System.Diagnostics;
using TranceSql.Processing;

namespace TranceSql.Oracle;

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
    public OracleDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper)
        : this(connectionFactory, parameterMapper, null)
    {
    }

    /// <summary>
    /// Creates command parameters for an Oracle database reference.
    /// </summary>
    /// <param name="connectionFactory">
    /// A connection factory that returns an Oracle DB connection.
    /// </param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public OracleDatabase(IConnectionFactory connectionFactory, ActivitySource? activitySource)
        : this(connectionFactory, null, activitySource)
    {
    }

    /// <summary>
    /// Creates command parameters for an Oracle database reference.
    /// </summary>
    /// <param name="connectionFactory">
    /// A connection factory that returns an Oracle DB connection.
    /// </param>
    /// <param name="parameterMapper">The parameter mapper.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public OracleDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ActivitySource? activitySource)
        : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), activitySource), new OracleDialect())
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
    public OracleDatabase(string connectionString, IParameterMapper? parameterMapper)
        : this(connectionString, parameterMapper, null)
    {
    }


    /// <summary>
    /// Creates command parameters for a Oracle database reference.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public OracleDatabase(string connectionString, ActivitySource? activitySource)
        : this(connectionString, null, activitySource)
    {
    }


    /// <summary>
    /// Creates command parameters for a Oracle database reference.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="parameterMapper">The parameter mapper.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public OracleDatabase(string connectionString, IParameterMapper? parameterMapper, ActivitySource? activitySource)
        : base(new SqlCommandManager(new OracleConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), activitySource), new OracleDialect())
    {
    }
}