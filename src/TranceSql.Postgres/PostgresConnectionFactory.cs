using Npgsql;
using System.Data.Common;

namespace TranceSql.Postgres;

/// <summary>
/// Basic <see cref="IConnectionFactory"/> that returns <see cref="NpgsqlConnection"/> instances.
/// </summary>
public class PostgresConnectionFactory : IConnectionFactory
{
    private string _connectionString;

    /// <summary>
    /// Creates a new factory using the provided connection string.
    /// </summary>
    /// <param name="connectionString">Connection string for this factory.</param>
    public PostgresConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc />
    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}