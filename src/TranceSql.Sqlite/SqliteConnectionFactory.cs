using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace TranceSql.Sqlite
{
    /// <summary>
    /// Basic <see cref="IConnectionFactory"/> that returns <see cref="SqliteConnection"/> instances.
    /// </summary>
    public class SqliteConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new factory using the provided connection string.
        /// </summary>
        /// <param name="connectionString">Connection string for this factory.</param>
        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public DbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}