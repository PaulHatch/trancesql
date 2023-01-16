using System;
using System.Collections.Generic;

namespace TranceSql;

/// <summary>
/// Represents a SQL function call.
/// </summary>
public class Function : ExpressionElement, ISqlElement
{
    /// <summary>
    /// Gets the function name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the function parameter arguments.
    /// </summary>
    public ICollection<ISqlElement> Parameters { get; } = new List<ISqlElement>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Function"/> class.
    /// </summary>
    /// <param name="name">The function name.</param>
    /// <exception cref="NullReferenceException">name</exception>
    public Function(string name)
    {
        Name = name ?? throw new NullReferenceException(nameof(name));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Function"/> class.
    /// </summary>
    /// <param name="name">The function name.</param>
    /// <param name="parameters">The parameter arguments for the function call.</param>
    /// <exception cref="NullReferenceException">name</exception>
    public Function(string name, params ISqlElement[] parameters)
    {
        Name = name ?? throw new NullReferenceException(nameof(name));
        (Parameters as List<ISqlElement>)!.AddRange(parameters);
    }

    void ISqlElement.Render(RenderContext context)
    {
        context.Write(Name);
        context.Write('(');
        context.RenderDelimited(Parameters);
        context.Write(')');
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}