using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TranceSql
{
    /// <summary>
    /// Represents a mechanism for managing a connection string which
    /// periodically rotates, for example using credentials from a Vault
    /// service. This is an abstract base class, use a vendor specific
    /// implementation from one of the driver packages.
    /// </summary>
    public abstract class RollingCredentials
    {
        private readonly string _baseConnectionString;
        private readonly Func<Task<ExpiringCredentials>> _refreshCredentials;
        private readonly Func<string, ExpiringCredentials, string> _createConnectionString;
        private ExpiringCredentials _credentials;
        private string _connectionString;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingCredentials" /> class.
        /// </summary>
        /// <param name="baseConnectionString">The base connection string.</param>
        /// <param name="refreshCredentials">A method to retrieve credentials.</param>
        protected RollingCredentials(
            string baseConnectionString,
            Func<Task<ExpiringCredentials>> refreshCredentials)
        {
            _baseConnectionString = baseConnectionString;
            _refreshCredentials = refreshCredentials;
        }

        /// <summary>
        /// Produces a connection string using the new credentials.
        /// </summary>
        /// <param name="connectionString">The original connection string.</param>
        /// <param name="credentials">The new credentials to use.</param>
        /// <returns>
        /// A new connection string.
        /// </returns>
        protected abstract string WithNewCredentials(string connectionString, ExpiringCredentials credentials);

        /// <summary>
        /// Gets a connection string for these credentials.
        /// </summary>
        /// <returns>Connection string for this credentials set.</returns>
        public async Task<string> GetConnectionStringAsync()
        {
            if (_credentials.IsExpired)
            {
                try
                {
                    await _semaphore.WaitAsync();
                    if (_credentials.IsExpired)
                    {
                        var credentials = await _refreshCredentials();
                        _connectionString = _createConnectionString(_connectionString, credentials);
                        _credentials = credentials;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return _connectionString;
        }
    }
}
