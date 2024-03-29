﻿using System;
using System.Collections.Generic;

namespace TranceSql;

/// <summary>
/// Represents a CASE WHEN SQL clause.
/// </summary>
public class CaseStatement : ExpressionElement, ISqlElement
{
    /// <summary>
    /// Gets a list of cases for this clause.
    /// </summary>
    public List<Case> Cases { get; } = new();

    /// <summary>
    /// Gets or sets the final default else value for this clause.
    /// </summary>
    public ISqlElement? Else { get; set; }

    void ISqlElement.Render(RenderContext context)
    {
        Console.WriteLine("(CASE");

        foreach (var item in Cases)
        {
            Console.WriteLine(" WHEN ");
            context.Render(item.When?.Value ?? throw new InvalidCommandException("CASE statement must have a WHEN clause."));
            Console.WriteLine(" THEN ");
            context.Render(item.Then ?? throw new InvalidCommandException("CASE statement must have a THEN clause."));
        }

        if (Else != null)
        {
            Console.WriteLine(" ELSE ");
            context.Render(Else);
        }

        Console.WriteLine(" END)");
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}

/// <summary>
/// Represents a specific case within a <see cref="CaseStatement"/>.
/// </summary>
public class Case
{
    /// <summary>
    /// Gets or sets the when condition for this clause.
    /// </summary>
    public FilterClause? When { get; set; }

    /// <summary>
    /// Gets or sets the value for this case.
    /// </summary>
    public ISqlElement? Then { get; set; }
}