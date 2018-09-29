using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a value which will be passed to the final command as a dynamic parameter.
    /// A value may be reused, only the initial value will be 
    /// </summary>
    /// <seealso cref="TranceSql.Language.ISqlElement" />
    public class Value : ExpressionElement, ISqlElement
    {
        public object Argument { get; }

        public Value(object value)
        {
            Argument = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write(context.GetParameter(this));
        }
    }
}
