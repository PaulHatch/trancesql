using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    public class Delete : ISqlStatement
    {
        private AnyOf<Table, Alias, ISqlElement> _from;
        public Any<Table, Alias, string> From
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
                context.Write("DELETE FROM ");
                context.Render(_from.Value);
                if (_where?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("WHERE ");
                    context.Render(_where);
                }
                context.Write(';');
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}
