using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql.Oracle
{
    /// <summary>
    /// Represents a mechanism for managing a connection string for a Oracle
    /// database which periodically rotates credentials, for example using
    /// credentials from a Vault service.
    /// </summary>
    public class OracleRollingCredentials : RollingCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleRollingCredentials"/> class.
        /// </summary>
        /// <param name="baseConnectionString">The base connection string.</param>
        /// <param name="refreshCredentials">A method to retrieve credentials.</param>
        public OracleRollingCredentials(
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
            return new OracleConnectionStringBuilder(connectionString)
            {
                UserID = credentials.Username,
                Password = credentials.Password
            }.ToString();
        }
    }
}
