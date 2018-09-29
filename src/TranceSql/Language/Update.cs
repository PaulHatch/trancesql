using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    public class Update : ISqlStatement
    {
        // Here we have a property of type Any<T1,T2> with an AnyOf<T1,T2,T3> used as a
        // backing field. This is a little bit complicated but it fulfills several criteria
        // - Allows string assignment implicitly converted to tables
        // - Performs table instantiation only once per assignment
        // - Restricts input to valid 
        // - Allows straightforward inspection and access to the table values by external code

        private AnyOf<Table, Alias, ISqlElement> _table;
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
        public AssignmentCollection Set
        {
            get => _set = _set ?? new AssignmentCollection();
            set => _set = value;
        }

        private ConditionCollection _where;
        public ConditionCollection Where
        {
            get => _where = _where ?? new ConditionCollection();
            set => _where = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.Nested))
            {
                context.Write("UPDATE ");
                context.Render(Table.Value as ISqlElement);
                context.WriteLine();
                context.Write("SET ");
                context.RenderDelimited(_set);

                if (_where?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("WHERE ");
                    Where.RenderCollection(context);
                }
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}
