 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql
{

    /// <summary>
    /// Represents an exception due to an invalid command structure.
    /// </summary>
    public class InvalidCommandException : InvalidOperationException
    {
        /// <summary>
        /// Represents an exception due to an invalid command.
        /// </summary>
        public InvalidCommandException() : base() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCommandException(string message) : base(message) { }

        /// <summary>
        /// Represents an exception due to an invalid command.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidCommandException(string message, Exception inner) : base(message, inner) { }        
    }
}
