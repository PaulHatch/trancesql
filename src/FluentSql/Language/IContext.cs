using System.Collections.Generic;

namespace TranceSql.Language
{
    public interface IContext
    {
        /// <summary>
        /// Gets the SQL command text for this context.
        /// </summary>
        string CommandText { get; }
        
        /// <summary>
        /// Gets the parameters for this context.
        /// </summary>
        IReadOnlyDictionary<string, object> ParameterValues { get; }
    }
}