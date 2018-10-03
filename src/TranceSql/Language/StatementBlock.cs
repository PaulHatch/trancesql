using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    public class StatementBlock : List<ISqlStatement>, ISqlStatement
    {
        void ISqlElement.Render(RenderContext context)
        {
            context.WriteLine("BEGIN");
            context.RenderDelimited(this, context.LineDelimiter);
            context.WriteLine();
            context.Write("END");
        }

        public override string ToString() => this.RenderDebug();
    }
}
