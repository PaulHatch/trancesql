using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    public class Insert : ISqlStatement
    {
        public Table Into { get; set; }

        public ColumnCollection _columns;
        public ColumnCollection Columns
        {
            get => _columns = _columns ?? new ColumnCollection();
            set => _columns = value;
        }

        public ValuesCollection _values;
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
                    context.RenderDelimited(Columns);
                    context.Write(')');
                }

                context.WriteLine();

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

                    if(!isClosed)
                    {
                        context.Write(')');
                    }
                    
                }

                context.Write(';');
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}
