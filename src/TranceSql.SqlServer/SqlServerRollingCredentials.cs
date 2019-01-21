using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql.SqlServer
{
    /// <summary>
    /// Represents a mechanism for managing a connection string for a SQL Server
    /// database which periodically rotates credentials, for example using
    /// credentials from a Vault service.
    /// </summary>
    public class SqlServerRollingCredentials : RollingCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerRollingCredentials"/> class.
        /// </summary>
        /// <param name="baseConnectionString">The base connection string.</param>
        /// <param name="refreshCredentials">A method to retrieve credentials.</param>
        public SqlServerRollingCredentials(
            string baseConnectionString,
            Func<Task<ExpiringCredentials>> refreshCredentials)
            : base(baseConnectionString, refreshCredentials)
        {
        }

        /// <summary>
        /// Produces a connection string using the new credentials.
        /// </summary>
        /// <param name="connectionString">The original connection string.</param>
        /// <param name="credentials">The new credentials to use.</param>
        /// <returns>A new connection string.</returns>
        protected override string WithNewCredentials(string connectionString, ExpiringCredentials credentials)
        {
            return new SqlConnectionStringBuilder(connectionString)
            {
                UserID = credentials.Username,
                Password = credentials.Password
            }.ToString();
        }
    }
}
