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
        public Any<Table, Alias, string> Table
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

        /// <summary>
        /// Gets or sets the condition filter this statement applies to.
        /// </summary>
        public AnyOf<Condition, ConditionPair, ICondition> Where { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.Nested))
            {
                context.Write("UPDATE ");
                context.Render(Table.Value as ISqlElement);
                context.WriteLine();
                context.Write("SET ");
                context.RenderDelimited(_set);

                if (Where != null)
                {
                    context.WriteLine();
                    context.Write("WHERE ");
                    context.Render(Where.Value);
                }
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
