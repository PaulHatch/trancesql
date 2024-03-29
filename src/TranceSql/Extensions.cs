﻿namespace TranceSql;

/// <summary>
/// SQL language extensions.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Creates an alias for the specified element.
    /// </summary>
    /// <param name="element">The element to alias.</param>
    /// <param name="alias">The alias name.</param>
    /// <returns>A new <see cref="Alias"/> instance</returns>
    public static Alias As(this ISqlElement element, string alias)
        => new(element, alias);

    /// <summary>
    /// Creates an ascending <see cref="Order" /> instance using the specified element.
    /// </summary>
    /// <param name="element">The element to order.</param>
    /// <returns>A new <see cref="Order"/> element.</returns>
    public static Order Asc(this ISqlElement element)
        => new(element, Direction.Ascending);

    /// <summary>
    /// Creates a descending <see cref="Order" /> instance using the specified element.
    /// </summary>
    /// <param name="element">The element to order.</param>
    /// <returns>A new <see cref="Order"/> element.</returns>
    public static Order Desc(this ISqlElement element)
        => new(element, Direction.Descending);

    /// <summary>
    /// Renders an element to a string using default debug settings.
    /// </summary>
    /// <param name="element">The element to render.</param>
    /// <returns>String representing the specified element.</returns>
    internal static string RenderDebug(this ISqlElement element)
    {
        var debugContext = new RenderContext(new GenericDialect());
        element.Render(debugContext);
        return debugContext.CommandText;
    }

    /// <summary>
    /// Renders an element to a string using default debug settings. This
    /// method can be accessed by external libraries which need include
    /// ISqlElement implementations.
    /// </summary>
    /// <param name="element">The element to render.</param>
    /// <returns>String representing the specified element.</returns>
    public static string CreateDebugString(ISqlElement element) => element.RenderDebug();
}