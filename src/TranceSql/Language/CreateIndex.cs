using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class CreateIndex : ISqlStatement
    {
        public bool Unique { get; }
        public string Name { get; set; }

        public Table On { get; set; }

        public ColumnOrderCollection Columns { get; } = new ColumnOrderCollection();

        void ISqlElement.Render(RenderContext context)
        {
            if (Unique)
            {
                context.Write("CREATE UNIQUE INDEX ");
            }
            else
            {
                context.Write("CREATE INDEX ");
            }

            context.WriteIdentifier(Name);
            context.WriteLine();
            context.Write("ON ");
            context.Render(On);
            context.Write('(');
            context.RenderDelimited(Columns);
            context.Write(");");
        }
    }
}
