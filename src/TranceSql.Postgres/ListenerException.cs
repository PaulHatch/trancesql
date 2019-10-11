using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Represents an error that occurs while a listener is running.
    /// </summary>
    public class ListenerException : DbException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerException"/> class.
        /// </summary>
        /// <param name="message">The message to display for this exception.</param>
        public ListenerException(string message) : base(message)
        {
        }
    }
}