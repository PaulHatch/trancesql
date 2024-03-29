﻿using System.Linq;

namespace TranceSql;

/// <summary>
/// Represents an INSERT statement
/// </summary>
public class Insert : ISqlStatement
{
    /// <summary>
    /// Gets or sets the table to insert into.
    /// </summary>
    public Table? Into { get; set; }

    private ColumnCollection? _columns;
    /// <summary>
    /// Gets or sets the insert columns.
    /// </summary>
    public ColumnCollection Columns
    {
        get => _columns ??= new ColumnCollection();
        set => _columns = value;
    }

    private ValuesCollection? _values;
    /// <summary>
    /// Gets or sets the values to be inserted. This value can be
    /// assigned a <see cref="Select"/> statement as well as values.
    /// </summary>
    public ValuesCollection Values
    {
        get => _values ??= new ValuesCollection();
        set => _values = value;
    }

    private ColumnCollection? _returning;
    /// <summary>
    /// Gets or sets the columns to return/output.
    /// </summary>
    public ColumnCollection? Returning
    {
        get => _returning ??= new ColumnCollection();
        set => _returning = value;
    }

    void ISqlElement.Render(RenderContext context)
    {
        var hasReturnValues = _returning?.Any() == true;
        if (hasReturnValues && context.Dialect.OutputType == OutputType.None)
        {
            throw new InvalidCommandException("This dialect does not support return clauses in insert statements.");
        }

        using (context.EnterChildMode(RenderMode.Nested))
        {
            // Check if columns were explicitly provided, if so we can automatically
            // determine where new value rows should go and add commas and parentheses.
            var hasColumns = _columns?.Any() == true;

            context.Write("INSERT INTO ");
            context.Render(Into ?? throw new InvalidCommandException("No table was specified for the insert statement."));

            if (hasColumns)
            {
                context.Write(" (");
                context.RenderDelimited(Columns, columnNamesOnly: true);
                context.Write(')');
            }

            context.WriteLine();
            if (hasReturnValues && context.Dialect.OutputType == OutputType.Output)
            {
                context.Write("OUTPUT ");
                context.RenderDelimited(_returning);
                context.WriteLine();
            }

            if (_values?.Any() != true)
            {
                context.Write("DEFAULT VALUES;");
                return;
            }

            if (!_values.IsSelect)
            {
                context.Write("VALUES ");
            }

            if (Values.IsSelect)
            {
                context.Render(Values.Single());
            }
            else
            {
                var cols = hasColumns ? (int?)_columns!.Count : null; // # of columns
                var valueIndex = 0; // track the offset for each value in the row
                var first = true; // track if we're on the first value
                var isClosed = true; // track if we left a parentheses open

                foreach (var value in Values)
                {
                    if (value is Values)
                    {
                        if (!first)
                        {
                            // Comma + newline between value groups
                            context.WriteLine(",");
                        }
                        context.Render(value);
                        valueIndex = 0;
                    }
                    else
                    {
                        if (valueIndex == 0) // start of values group
                        {
                            if (!first) // first value in entire collection
                            {
                                // Comma + newline between value groups
                                context.WriteLine(",");
                            }
                            // Open new value group
                            context.Write('(');
                            isClosed = false;
                        }
                        else
                        {
                            // comma between values
                            context.Write(", ");
                        }

                        context.Render(value);
                        valueIndex++;

                        // add a closing ')' if we're matching the number of columns
                        if (valueIndex == cols)
                        {
                            context.Write(")");
                            isClosed = true;
                            valueIndex = 0;
                        }
                    }

                    first = false;
                }

                if (!isClosed)
                {
                    context.Write(')');
                }
            }
        }

        if (context.Mode != RenderMode.MultiStatment)
        {
            if (hasReturnValues && context.Dialect.OutputType == OutputType.Returning)
            {
                context.WriteLine();
                context.Write("RETURNING ");
                context.RenderDelimited(_returning);
            }

            context.Write(';');
        }
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}