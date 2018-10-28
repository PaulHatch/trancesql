using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Processing
{
    /// <summary>
    /// Holds information about a specific database connection that will
    /// be used for creating traces and debug information.
    /// </summary>
    public class DbInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbInfo"/> class.
        /// </summary>
        /// <param name="server">The database host specified in the connection string.</param>
        /// <param name="database">The default database specified in the connection string.</param>
        /// <param name="user">The user specified in the connection string.</param>
        public DbInfo(string server, string database, string user)
        {
            Server = server;
            Database = database;
            User = user;
        }

        /// <summary>
        /// Gets the database host specified in the connection string.
        /// </summary>
        public string Server { get; }

        /// <summary>
        /// Gets the default database specified in the connection string.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Gets the user specified in the connection string.
        /// </summary>
        public string User { get; }
    }
}
