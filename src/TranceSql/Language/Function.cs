using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class Function : ExpressionElement, ISqlElement
    {
        public string Name { get; }
        public ICollection<ISqlElement> Parameters { get; } = new List<ISqlElement>();

        public Function(string name)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
        }

        public Function(string name, params ISqlElement[] parameters)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            (Parameters as List<ISqlElement>).AddRange(parameters);
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write(Name);
            context.Write('(');
            context.RenderDelimited(Parameters);
            context.Write(')');
        }
    }
}
