namespace TranceSql;

/// <summary>
/// Helper class, intended for use with C#'s <code>using static</code> to provide
/// quick access to helper methods such as <see cref="UsingStatic.Column(string)"/>
/// for creating SQL elements.
/// </summary>
public static class UsingStatic
{
    /// <summary>
    /// Creates a Column element.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <returns>
    /// A new Column instance.
    /// </returns>
    public static Column Column(string name) => new(name);

    /// <summary>
    /// Creates a Column element.
    /// </summary>
    /// <param name="table">The column's table name.</param>
    /// <param name="name">The column name.</param>
    /// <returns>
    /// A new Column instance.
    /// </returns>
    public static Column Column(string table, string name) => new(table, name);
        
    /// <summary>
    /// Creates a Column element.
    /// </summary>
    /// <param name="schema">The column's tables' schema name.</param>
    /// <param name="table">The column's table name.</param>
    /// <param name="name">The column name.</param>
    /// <returns>A new Column instance.</returns>
    public static Column Column(string schema, string table, string name) => new(schema, table, name);

    /// <summary>
    /// Creates a Table element.
    /// </summary>
    /// <param name="table">The table's name.</param>
    /// <returns>
    /// A new Table instance.
    /// </returns>
    public static Table Table(string table) => new(table);
        
    /// <summary>
    /// Creates a Table element.
    /// </summary>
    /// <param name="schema">The table's schema name.</param>
    /// <param name="table">The table's name.</param>
    /// <returns>
    /// A new Table instance.
    /// </returns>
    public static Table Table(string schema, string table) => new(schema, table);

    /// <summary>
    /// Creates a Parameter element.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns>
    /// A new Parameter instance.
    /// </returns>
    public static Parameter Parameter(string name) => new(name);

    /// <summary>
    /// Creates a new Value instance.
    /// </summary>
    /// <param name="value">The value to create the instance for.</param>
    /// <returns>A new Value instance.</returns>
    public static Value Value(object? value) => new(value);

    /// <summary>
    /// Creates a new Values instance.
    /// </summary>
    /// <param name="value">A single value for the set.</param>
    /// <returns>A new Values instance.</returns>
    public static Values Values(object? value) => new() { value };
        
    /// <summary>
    /// Creates a new Values instance.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <returns>A new Values instance.</returns>
    public static Values Values(object? first, object? second) => new() { first, second };
        
    /// <summary>
    /// Creates a new Values instance.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <param name="third">The third value.</param>
    /// <returns>A new Values instance.</returns>
    public static Values Values(object? first, object? second, object? third) => new() { first, second, third };

    /// <summary>
    /// Creates a new Values instance.
    /// </summary>
    /// <param name="values">The values for this set.</param>
    /// <returns>
    /// A new Values instance.
    /// </returns>
    public static Values Values(params object?[] values) => new(values);

    /// <summary>
    /// Creates a function for COUNT(*).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Count() => new("COUNT", new Column("*"));
        
    /// <summary>
    /// Creates a function for MAX({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Max(ISqlElement element) => new("MAX", element);
        
    /// <summary>
    /// Creates a function for MIN({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Min(ISqlElement element) => new("MIN", element);
        
    /// <summary>
    /// Creates a function for SUM({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Sum(ISqlElement element) => new("SUM", element);

    /// <summary>
    /// Creates a function for COALESCE({elements...}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Coalesce(params ISqlElement[] elements) => new("COALESCE", elements);

    /// <summary>
    /// Creates a function for AVG({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Avg(ISqlElement element) => new("AVG", element);
        
    /// <summary>
    /// Creates a function for ABS({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Abs(ISqlElement element) => new("ABS", element);
        
    /// <summary>
    /// Creates a function for CEILING({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Ceiling(ISqlElement element) => new("CEILING", element);

    /// <summary>
    /// Creates a function for FLOOR({element}).
    /// </summary>
    /// <returns>A new Function instance.</returns>
    public static Function Floor(ISqlElement element) => new("FLOOR", element);

}