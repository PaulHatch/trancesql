using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql;

/// <summary>
/// Represents the context of a command being rendered for execution.
/// </summary>
public class RenderContext : IContext
{
    private int _index = 1;

    private Dictionary<Value, string> _dynamicParameters = new();
    private Stack<RenderMode> _modes = new(new[] { RenderMode.Statment });
    private StringBuilder _result = new();
    private DeferContext? _deferContext;

    /// <summary>
    /// Gets or sets the line delimiter to use when rendering commands.
    /// </summary>
    public string LineDelimiter { get; set; } = "\n";

    // Yes, it is entirely possible that this is overkill, but just mutating the render mode didn't feel right. The
    // mode handler allows creating nested render modes for the context by calling "EnterChildMode" inside of a using block.
    private class ModeHandler : IDisposable
    {
        private Stack<RenderMode> _modes;
        public ModeHandler(Stack<RenderMode> modes, RenderMode mode) { _modes = modes; _modes.Push(mode); }
        public void Dispose() { if (_modes.Any()) { _modes.Pop(); } }
    }

    /// <summary>
    /// Gets or sets the name of the operation to be used for recording
    /// tracing information.
    /// </summary>
    public string? OperationName { get; set; }

    /// <summary>
    /// Creates a child mode context which will return to the previous mode when disposed.
    /// </summary>
    /// <param name="mode">The new mode to use.</param>
    /// <returns>A mode context</returns>
    public IDisposable EnterChildMode(RenderMode mode) => new ModeHandler(_modes, mode);

    /// <summary>
    /// Gets the named parameters for this context.
    /// </summary>
    internal Dictionary<string, object?>? NamedParameters { get; set; }

    /// <summary>
    /// Gets the current rendering mode.
    /// </summary>
    public RenderMode Mode => _modes.Peek();

    /// <summary>
    /// Gets the SQL command text for this context.
    /// </summary>
    public string CommandText => _result.ToString();

    /// <summary>
    /// Gets the parameters for this context.
    /// </summary>
    public IReadOnlyDictionary<string, object?> ParameterValues
    {
        get
        {
            var results = _dynamicParameters
                .Select(v => (name: v.Value, value: v.Key.Argument));

            if (NamedParameters != null)
            {
                results = results
                    .Concat(NamedParameters
                        .Select(v => (name: v.Key, value: v.Value)));
            }

            return results.ToDictionary(v => v.name, v => v.value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderContext"/> class.
    /// </summary>
    /// <param name="dialect">The dialect to render in.</param>
    public RenderContext(IDialect dialect)
    {
        Dialect = dialect;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderContext" /> class.
    /// </summary>
    /// <param name="dialect">The dialect to render in.</param>
    /// <param name="deferContext">The context for deferred commands.</param>
    public RenderContext(IDialect dialect, DeferContext? deferContext)
    {
        Dialect = dialect;
        _deferContext = deferContext;
    }

    /// <summary>
    /// Gets a parameter for the specified value and registers the value with this context..
    /// </summary>
    /// <param name="value">The value to get a parameter for.</param>
    /// <returns>The name of the parameter for the specified value.</returns>
    public string GetParameter(Value value)
    {
        if (_dynamicParameters.ContainsKey(value))
        {
            return _dynamicParameters[value];
        }

        string name;
        if (_deferContext == null)
        {
            name = $"@P{_index}";
            _index++;
        }
        else
        {
            name = $"@P{_deferContext.ParameterIndex}";
            _deferContext.ParameterIndex++;
        }

        _dynamicParameters.Add(value, name);
        return name;
    }

    /// <summary>
    /// Gets the SQL dialect for this render context.
    /// </summary>
    public IDialect Dialect { get; }

    /// <summary>
    /// Renders the specified element, this is provided for convenience as
    /// ISqlElements generally implement render explicitly on ISqlElement.
    /// </summary>
    /// <param name="element">The element to render.</param>
    public void Render(ISqlElement element)
    {
        element.Render(this);
    }

    /// <summary>
    /// Writes the specified string to the rendered output.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void Write(string value) => _result.Append(value);

    /// <summary>
    /// Writes the specified char to the rendered output.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(char value) => _result.Append(value);

    /// <summary>
    /// Writes a string using the current dialect's identifier formating rules
    /// followed by a '.' mark. if the string provided isn't empty, no action 
    /// is taken.
    /// </summary>
    /// <param name="value">The identifier to write.</param>
    public void WriteIdentifierPrefix(string? value)
    {
        if (value is null or "") return;
        _result.Append(Dialect.FormatIdentifier(value));
        _result.Append('.');
    }

    /// <summary>
    /// Writes a string using the current dialect's identifier formating rules.
    /// </summary>
    /// <param name="value">The identifier to write.</param>
    public void WriteIdentifier(string value)
    {
        _result.Append(Dialect.FormatIdentifier(value));
    }

    /// <summary>
    /// Writes a line using the configured line delimiter.
    /// </summary>
    public void WriteLine()
    {
        _result.Append(LineDelimiter);
    }

    /// <summary>
    /// Writes a line using the configured line delimiter.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteLine(string value)
    {
        _result.Append(value);
        _result.Append(LineDelimiter);
    }

    /// <summary>
    /// Renders the specified elements delimited by the specified delimiter.
    /// </summary>
    /// <param name="items">The items to render.</param>
    /// <param name="delimiter">The delimiter string to use.</param>
    /// <param name="columnNamesOnly">
    /// If true only the name of any column in the specified list will be used.
    /// </param>
    public void RenderDelimited(
        IEnumerable<ISqlElement>? items,
        string delimiter = ", ",
        bool columnNamesOnly = false)
    {
        if (items == null)
        {
            return;
        }

        var first = true;
        foreach (var item in items)
        {
            if (!first)
            {
                _result.Append(delimiter);
            }
            else
            {
                first = false;
            }

            if (columnNamesOnly && item is Column column)
            {
                WriteIdentifier(column.Name);
            }
            else
            {
                item.Render(this);
            }
        }

    }
}