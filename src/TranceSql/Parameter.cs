using System;

namespace TranceSql;

/// <summary>
/// Represents a parameter in a SQL statement. For automatic parameter
/// creation, use the <see cref="Value"/> element.
/// </summary>
public class Parameter : ExpressionElement, ISqlElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="name">
    /// The name. The name will automatically be prefixed with '@' if none is present.
    /// </param>
    public Parameter(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    public Parameter() { }

    private string? _name;
    /// <summary>
    /// Gets or sets the name. The name will automatically be prefixed with 
    /// '@' if none is present.
    /// </summary>
    public string? Name
    {
        get => _name;
        set
        {
            if (_name == value)
            {
                throw new ArgumentNullException(nameof(value), "Parameter name cannot be null");
            }
            _name = value?.StartsWith("@") == true ? value : $"@{value}";
        }
    }

    internal string GetRequiredName()
    {
        return _name ?? throw new InvalidCommandException("Name cannot be null");
    }

    void ISqlElement.Render(RenderContext context)
    {
        context.Write(Name ?? throw new InvalidOperationException("Parameter name cannot be null"));
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}