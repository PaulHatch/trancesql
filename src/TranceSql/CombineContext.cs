using System.Collections.Generic;
using System.Text;
using TranceSql.Language;

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
        /// Appends the specified context result to this context.
        /// </summary>
        /// <param name="context">The context to be appended.</param>
        public void Append(IContext context)
        {
            _commandText.Append(context.CommandText);
            foreach (var parameter in context.ParameterValues)
            {
                _parameters.Add(parameter.Key, parameter.Value);
            }
            
        }
    }
}