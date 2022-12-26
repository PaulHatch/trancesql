using System.Data.Common;
using MySqlConnector;

namespace TranceSql.MySql
{
    /// <summary>
    /// Basic <see cref="IConnectionFactory"/> that returns <see cref="MySqlConnection"/> instances.
    /// </summary>
    public class MySqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new factory using the provided connection string.
        /// </summary>
        /// <param name="connectionString">Connection string for this factory.</param>
        public MySqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public DbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
