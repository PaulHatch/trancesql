using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Thrown when a parameter is added to a transaction which already
    /// contains a parameter of the same name but a different value.
    /// </summary>
    public class ParameterValuesMismatchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterValuesMismatchException"/> class.
        /// </summary>
        public ParameterValuesMismatchException() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterValuesMismatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParameterValuesMismatchException(string message) : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterValuesMismatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ParameterValuesMismatchException(string message, Exception inner) : base(message, inner) { }
    }
}
