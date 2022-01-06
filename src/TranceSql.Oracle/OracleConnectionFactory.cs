using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace TranceSql.Oracle
{
    /// <summary>
    /// Basic <see cref="IConnectionFactory"/> that returns <see cref="OracleConnection"/> instances.
    /// </summary>
    public class OracleConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new factory using the provided connection string.
        /// </summary>
        /// <param name="connectionString">Connection string for this factory.</param>
        public OracleConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public DbConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }
}