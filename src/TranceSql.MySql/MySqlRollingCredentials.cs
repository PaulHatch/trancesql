using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql.MySql
{
    /// <summary>
    /// Represents a mechanism for managing a connection string for a MySQL
    /// database which periodically rotates credentials, for example using
    /// credentials from a Vault service.
    /// </summary>
    public class MySqlRollingCredentials : RollingCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlRollingCredentials"/> class.
        /// </summary>
        /// <param name="baseConnectionString">The base connection string.</param>
        /// <param name="refreshCredentials">A method to retrieve credentials.</param>
        public MySqlRollingCredentials(
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
            return new MySqlConnectionStringBuilder(connectionString)
            {
                UserID = credentials.Username,
                Password = credentials.Password
            }.ToString();
        }
    }
}
