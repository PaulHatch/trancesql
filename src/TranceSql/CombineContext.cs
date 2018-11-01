using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// A special non-public context type used for combining multiple context results
    /// into a single executable context.
    /// </summary>
    internal class CombineContext : IContext
    {
        StringBuilder _commandText = new StringBuilder();
        Dictionary<string, object> _parameters = new Dictionary<string, object>();

        /// <summary>
        /// Gets the SQL command text for this context.
        /// </summary>
        public string CommandText => _commandText.ToString();

        /// <summary>
        /// Gets the parameters for this context.
        /// </summary>
        public IReadOnlyDictionary<string, object> ParameterValues => _parameters;

        /// <summary>
        /// Gets or sets the name of the operation to be used for recording
        /// tracing information.
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Appends the specified context result to this context.
        /// </summary>
        /// <param name="context">The context to be appended.</param>
        public void Append(IContext context)
        {
            if (!String.IsNullOrEmpty(context.OperationName))
            {
                if (String.IsNullOrEmpty(OperationName))
                {
                    OperationName = context.OperationName;
                }
                else if (!String.IsNullOrEmpty(context.OperationName))
                {
                    OperationName = $"{OperationName}+{context.OperationName}";
                }
            }

            _commandText.Append(context.CommandText);
            foreach (var parameter in context.ParameterValues)
            {
                _parameters.Add(parameter.Key, parameter.Value);
            }
            
        }
    }
}