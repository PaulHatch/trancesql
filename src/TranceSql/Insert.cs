using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents an INSERT statement
    /// </summary>
    public class Insert : ISqlStatement
    {
        /// <summary>
        /// Gets or sets the table to insert into.
        /// </summary>
        public Table Into { get; set; }

        private ColumnCollection _columns;
        /// <summary>
        /// Gets or sets the insert columns.
        /// </summary>
        public ColumnCollection Columns
        {
            get => _columns = _columns ?? new ColumnCollection();
            set => _columns = value;
        }

        private ValuesCollection _values;
        /// <summary>
        /// Gets or sets the values to be inserted. This value can be
        /// assigned a <see cref="Select"/> statement as well as values.
        /// </summary>
        public ValuesCollection Values
        {
            get => _values = _values ?? new ValuesCollection();
            set => _values = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.Nested))
            {
                // Check if columns were explicitly provided, if so we can automatically
                // determine where new value rows should go and add commas and parentheses.
                var hasColumns = _columns?.Any() == true;

                context.Write("INSERT INTO ");
                context.Render(Into);

                if (hasColumns)
                {
                    context.Write(" (");
                    context.RenderDelimited(Columns, columnNamesOnly: true);
                    context.Write(')');
                }

                context.WriteLine();

                if (_values?.Any() != true)
                {
                    context.Write("DEFAULT VALUES;");
                }
                else
                {
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
                        var cols = hasColumns ? (int?)_columns.Count : null; // # of columns
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
            }

            if (context.Mode != RenderMode.MultiStatment)
            {
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
}
