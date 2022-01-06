using System.Data.Common;
using System.Data.SqlClient;

namespace TranceSql.SqlServer
{
    /// <summary>
    /// Basic <see cref="IConnectionFactory"/> that returns <see cref="SqlConnection"/> instances.
    /// </summary>
    public class SqlServerConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new factory using the provided connection string.
        /// </summary>
        /// <param name="connectionString">Connection string for this factory.</param>
        public SqlServerConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public DbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
