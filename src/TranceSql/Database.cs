using System;
using System.Collections.Generic;
using System.Text;
using TranceSql.Language;
using TranceSql.Processing;

namespace TranceSql
{
    /// <summary>
    /// Represents a specific database to connect a command to.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="manager">The SQL command manager.</param>
        /// <param name="dialect">The language dialect.</param>
        public Database(SqlCommandManager manager, IDialect dialect)
        {
            Manager = manager;
            Dialect = dialect;
        }

        /// <summary>
        /// Gets the command manager for this connection.
        /// </summary>
        public SqlCommandManager Manager { get; }

        /// <summary>
        /// Gets the dialect for this connection.
        /// </summary>
        public IDialect Dialect { get; }

        /// <summary>
        /// Creates a new context for running deferred commands using this connection's manager.
        /// </summary>
        /// <returns>A new <see cref="DeferContext"/> instance for this connection.</returns>
        public DeferContext CreateDeferContext() => Manager.CreateDeferContext();
    }
}
