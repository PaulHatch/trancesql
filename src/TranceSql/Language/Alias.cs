using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class Alias : 
        ISqlElement, 
        IDataSource // allow use in data source contexts such as FROM clause
    {
        public Alias(ISqlElement element, string alias)
        {
            Element = element;
            Name = alias;
        }

        public ISqlElement Element { get; }
        public string Name { get; }

        void ISqlElement.Render(RenderContext context)
        {
            Element.Render(context);
            context.Write($" AS {Name}");
        }

        public override string ToString() => this.RenderDebug();
    }
}
