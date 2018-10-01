using System.Collections.Generic;
using System.Linq;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents cached values from and already rendered command.
    /// </summary>
    internal class CachedContext : IContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedContext"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameterValues">The parameter values.</param>
        private CachedContext(string commandText, IReadOnlyDictionary<string, object> parameterValues)
        {
            CommandText = commandText;
            ParameterValues = parameterValues;
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
        /// Creates a new context based on this one with additional parameters added.
        /// </summary>
        /// <param name="additionalParameters">The additional parameters.</param>
        /// <returns></returns>
        public IContext WithParameters(IDictionary<string, object> additionalParameters)
        {
            return new CachedContext(
                CommandText,
                ParameterValues
                    .Concat(additionalParameters)
                    .ToDictionary(v => v.Key, v => v.Value)
            );
        }
    }
}