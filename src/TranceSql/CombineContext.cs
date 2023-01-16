using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql;

/// <summary>
/// A special non-public context type used for combining multiple context results
/// into a single executable context.
/// </summary>
internal class CombineContext : IContext
{
    private readonly StringBuilder _commandText = new();
    private readonly Dictionary<string, object?> _parameters = new();
        
    /// <summary>
    /// Tracks whether anything has been added to the context so that we can determine whether or not to
    /// execute the context.
    /// </summary>
    internal bool IncludesCommands { get; set; }

    /// <summary>
    /// Gets the SQL command text for this context.
    /// </summary>
    public string CommandText => _commandText.ToString();

    /// <summary>
    /// Gets the parameters for this context.
    /// </summary>
    public IReadOnlyDictionary<string, object?> ParameterValues => _parameters;

    /// <summary>
    /// Gets or sets the name of the operation to be used for recording
    /// tracing information.
    /// </summary>
    public string? OperationName { get; set; }

    /// <summary>
    /// Appends the specified context result to this context.
    /// </summary>
    /// <param name="context">The context to be appended.</param>
    public void Append(IContext context)
    {
        if (!string.IsNullOrEmpty(context.OperationName))
        {
            if (string.IsNullOrEmpty(OperationName))
            {
                OperationName = context.OperationName;
            }
            else if (!string.IsNullOrEmpty(context.OperationName))
            {
                OperationName = $"{OperationName}+{context.OperationName}";
            }
        }

        if (context.CommandText.Length != 0)
        {
            IncludesCommands = true;
        }

        _commandText.Append(context.CommandText);
        foreach (var parameter in context.ParameterValues)
        {
            _parameters.Add(parameter.Key, parameter.Value);
        }
            
    }
}