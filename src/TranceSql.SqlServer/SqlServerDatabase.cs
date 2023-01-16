using System.Diagnostics;
using TranceSql.Processing;

namespace TranceSql.SqlServer;

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
    public SqlServerDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper)
        : this(connectionFactory, parameterMapper, null)
    {
    }

    /// <summary>
    /// Creates command parameters for a Microsoft SQL Server database reference.
    /// </summary>
    /// <param name="connectionFactory">
    /// A connection factory that returns a SQL Server DB connection.
    /// </param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public SqlServerDatabase(IConnectionFactory connectionFactory, ActivitySource? activitySource)
        : this(connectionFactory, null, activitySource)
    {
    }

    /// <summary>
    /// Creates command parameters for a Microsoft SQL Server database reference.
    /// </summary>
    /// <param name="connectionFactory">
    /// A connection factory that returns a SQL Server DB connection.
    /// </param>
    /// <param name="parameterMapper">The parameter mapper.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public SqlServerDatabase(IConnectionFactory connectionFactory, IParameterMapper? parameterMapper, ActivitySource? activitySource)
        : base(new SqlCommandManager(connectionFactory, parameterMapper ?? new DefaultParameterMapper(), activitySource), new SqlServerDialect())
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
    public SqlServerDatabase(string connectionString, IParameterMapper? parameterMapper)
        : this(connectionString, parameterMapper, null)
    {
    }

    /// <summary>
    /// Creates command parameters for a Microsoft SQL Server database reference.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public SqlServerDatabase(string connectionString, ActivitySource? activitySource)
        : this(connectionString, null, activitySource)
    {
    }

    /// <summary>
    /// Creates command parameters for a Microsoft SQL Server database reference.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="parameterMapper">The parameter mapper.</param>
    /// <param name="activitySource">
    /// An activity source that can be used to create activities for database operations.
    /// </param>
    public SqlServerDatabase(string connectionString, IParameterMapper? parameterMapper, ActivitySource? activitySource)
        : base(new SqlCommandManager(new SqlServerConnectionFactory(connectionString), parameterMapper ?? new DefaultParameterMapper(), activitySource), new SqlServerDialect())
    {
    }
}