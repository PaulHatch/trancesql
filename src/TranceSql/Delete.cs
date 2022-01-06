using System.Linq;

namespace TranceSql
{
    /// <summary>
    /// Represents a DELETE FROM statement.
    /// </summary>
    public class Delete : ISqlStatement
    {
        private AnyOf<Table, Alias, ISqlElement> _from;
        
        /// <summary>
        /// Gets or sets table to delete from.
        /// </summary>
        public Any<Table, TableSchema, Alias, string> From
        {
            get
            {
                switch (_from?.Value)
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
                        _from = (Table)tableSchema;
                        break;
                    case string name:
                        _from = new Table(name);
                        break;
                    case Table table:
                        _from = table;
                        break;
                    case Alias alias:
                        _from = alias;
                        break;
                    default:
                        _from = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the where filter for this statement.
        /// </summary>
        public FilterClause Where { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            var output = OutputType.None;
            if (_returning?.Any() == true)
            {
                output = context.Dialect.OutputType;
                if (output == OutputType.None)
                {
                    throw new InvalidCommandException("This dialect does not support return clauses in delete statements.");
                }
            }

            using (context.EnterChildMode(RenderMode.Nested))
            {
                context.Write("DELETE FROM ");
                context.Render(_from.Value);
                
                // OUTPUT statements are rendered before where clauses
                if (output == OutputType.Output)
                {
                    context.WriteLine();
                    context.Write("OUTPUT ");
                    context.RenderDelimited(_returning);
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

                context.Write(';');
            }
        }

        private ColumnCollection _returning;
        /// <summary>
        /// Gets or sets the columns to return/output.
        /// </summary>
        public ColumnCollection Returning
        {
            get => _returning = _returning ?? new ColumnCollection();
            set => _returning = value;
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
