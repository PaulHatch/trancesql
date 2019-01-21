using System;

namespace TranceSql
{
    /// <summary>
    /// Represents username and password credentials for a database connection
    /// that uses a rolling credentials provider. This are provided
    /// </summary>
    public class ExpiringCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiringCredentials" /> class
        /// with a username and password with the specified lifespan.
        /// </summary>
        public ExpiringCredentials(string username, string password, TimeSpan validFor)
        {
            Username = username;
            Password = password;
            ExpiresAt = DateTimeOffset.Now + validFor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiringCredentials" /> class
        /// with a username and password with the specified absolute expiration
        /// time.
        /// </summary>
        public ExpiringCredentials(string username, string password, DateTimeOffset expiresAt)
        {
            Username = username;
            Password = password;
            ExpiresAt = expiresAt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiringCredentials" /> class
        /// with a username and password which does not expire.
        /// </summary>
        public ExpiringCredentials(string username, string password)
        {
            Username = username;
            Password = password;
            ExpiresAt = DateTimeOffset.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiringCredentials" /> class
        /// which has already expired, this can be used as an initial value if
        /// credentials have not yet been retrieved from a provider.
        /// </summary>
        private ExpiringCredentials()
        {
            ExpiresAt = DateTimeOffset.MinValue;
        }

        /// <summary>
        /// Gets a default empty credentials instance which have already expired,
        /// this can be used as an initial value if credentials have not yet been
        /// retrieved from a provider.
        /// </summary>
        public static ExpiringCredentials ExpiredCredentials { get; } = new ExpiringCredentials();

        /// <summary>Gets the username.</summary>
        public string Username { get; }
        /// <summary>Gets the password.</summary>
        public string Password { get; }
        /// <summary>Gets the expiration time for these credentials.</summary>
        public DateTimeOffset ExpiresAt { get; }

        /// <summary>
        /// Gets a value indicating whether these credentials have expired.
        /// </summary>
        public bool IsExpired => DateTimeOffset.Now >= ExpiresAt;
    }
}
