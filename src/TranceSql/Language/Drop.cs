using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class Drop : ISqlStatement
    {
        public Drop(Table table)
        {
            Type = DropType.Table;
            Target = table;
        }

        public Drop(DropType type, ISqlElement target)
        {
            Type = type;
            Target = target;
        }

        public Drop(DropType type, string target)
        {
            Type = type;
            switch (Type)
            {
                case DropType.Table:
                    Target = new Table(target);
                    break;
                default:
                    throw new InvalidCommandException($"Cannot convert string to drop type '{Type}'");
            }
        }

        public Drop(DropType type, string schema, string target)
        {
            Type = type;
            switch (Type)
            {
                case DropType.Table:
                    Target = new Table(schema, target);
                    break;
                default:
                    throw new InvalidCommandException($"Cannot convert schema+name strings to drop type '{Type}'");
            }
        }
        
        public DropType Type { get; }
        public ISqlElement Target { get; }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write("DROP ");
            switch (Type)
            {
                case DropType.Table:
                    context.Write("TABLE ");
                    break;
                case DropType.Constraint:
                    throw new NotImplementedException();
                default:
                    throw new InvalidCommandException($"Unrecognized drop type '{Type}'");
            }
            context.Render(Target);
            context.Write(';');
        }

        public override string ToString() => this.RenderDebug();
    }
}
