using System.Collections.Generic;
using System.Linq;

namespace TranceSql
{
    /// <summary>
    /// Represents cached values from and already rendered command.
    /// </summary>
    internal class CachedContext : IContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedContext" /> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="operationName">Name of the operation to use for tracing.</param>
        /// <param name="parameterValues">The parameter values.</param>
        private CachedContext(string commandText, string? operationName, IReadOnlyDictionary<string, object> parameterValues)
        {
            CommandText = commandText;
            ParameterValues = parameterValues;
            OperationName = operationName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedContext"/> class.
        /// </summary>
        /// <param name="context">The context to derive from.</param>
        public CachedContext(RenderContext context)
        {
            CommandText = context.CommandText;
            ParameterValues = context.ParameterValues;
        }

        /// <summary>
        /// Gets the SQL command text for this context.
        /// </summary>
        public string CommandText { get; }

        /// <summary>
        /// Gets the parameters for this context.
        /// </summary>
        public IReadOnlyDictionary<string, object> ParameterValues { get; }

        /// <summary>
        /// Gets or sets the name of the operation to be used for recording
        /// tracing information.
        /// </summary>
        public string? OperationName { get; set; }

        /// <summary>
        /// Creates a new context based on this one with additional parameters added.
        /// </summary>
        /// <param name="additionalParameters">The additional parameters.</param>
        /// <returns></returns>
        public IContext WithParameters(IDictionary<string, object> additionalParameters)
        {
            return new CachedContext(
                CommandText,
                OperationName,
                ParameterValues
                    .Concat(additionalParameters)
                    .ToDictionary(v => v.Key, v => v.Value)
            );
        }
    }
}