using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents an UPDATE TABLE statement.
    /// </summary>
    public class Update : ISqlStatement
    {
        // Here we have a property of type Any<T1,T2> with an AnyOf<T1,T2,T3> used as a
        // backing field. This is a little bit complicated but it fulfills several criteria
        // - Allows string assignment implicitly converted to tables
        // - Performs table instantiation only once per assignment
        // - Restricts input to valid 
        // - Allows straightforward inspection and access to the table values by external code

        private AnyOf<Table, Alias, ISqlElement> _table;
        /// <summary>
        /// Gets or sets the table to update. This can be set to a <see cref="Table"/>, a
        /// <see cref="Alias"/> of a table, or a <see cref="string"/> which will be converted
        /// to a <see cref="Table"/>.
        /// </summary>
        public Any<Table, TableSchema, Alias, string> Table
        {
            get
            {
                switch (_table?.Value)
                {
                    case Table table: return table;
                    case Alias alias: return alias;
                    default: return null;
                }
            }
            set
            {
                switch (value?.Value)
                {
                    case TableSchema tableSchema:
                        _table = (Table)tableSchema;
                        break;
                    case string name:
                        _table = new Table(name);
                        break;
                    case Table table:
                        _table = table;
                        break;
                    case Alias alias:
                        _table = alias;
                        break;
                    default:
                        _table = null;
                        break;
                }
            }
        }

        private AssignmentCollection _set;
        /// <summary>
        /// Gets or sets the assignment collection for this statement.
        /// </summary>
        public AssignmentCollection Set
        {
            get => _set = _set ?? new AssignmentCollection();
            set => _set = value;
        }

        private DataSourceCollection _from;
        /// <summary>
        /// Gets or sets table or tables for the from clause of this statement.
        /// </summary>
        public DataSourceCollection From
        {
            get => _from = _from ?? new DataSourceCollection();
            set => _from = value;
        }

        /// <summary>
        /// Gets or sets the condition filter this statement applies to.
        /// </summary>
        public FilterClause Where { get; set; }

        private ColumnCollection _returning;
        /// <summary>
        /// Gets or sets the columns to return/output.
        /// </summary>
        public ColumnCollection Returning
        {
            get => _returning = _returning ?? new ColumnCollection();
            set => _returning = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            var output = OutputType.None;
            if (_returning?.Any() == true)
            {
                output = context.Dialect.OutputType;
                if (output == OutputType.None)
                {
                    throw new InvalidCommandException("This dialect does not support return clauses in update statements.");
                }
            }

            using (context.EnterChildMode(RenderMode.Nested))
            {
                context.Write("UPDATE ");
                context.Render(Table.Value as ISqlElement);
                context.WriteLine();
                context.Write("SET ");
                context.RenderDelimited(_set);

                // OUTPUT statements are rendered before where clauses
                if (output == OutputType.Output)
                {
                    context.WriteLine();
                    context.Write("OUTPUT ");
                    context.RenderDelimited(_returning);
                }

                if (_from?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("FROM ");
                    context.RenderDelimited(From);
                }

                if (Where != null)
                {
                    context.WriteLine();
                    context.Write("WHERE ");
                    context.Render(Where.Value);
                }

                // RETURNING statements are rendered after where clauses
                if (output == OutputType.Returning)
                {
                    context.WriteLine();
                    context.Write("RETURNING ");
                    context.RenderDelimited(_returning);
                }
            }

            if (context.Mode == RenderMode.Statment)
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
