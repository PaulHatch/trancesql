using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a raw snippet of SQL code.
    /// </summary>
    public class Raw : ExpressionElement, ISqlElement
    {
        public string Value { get; set; }

        public Raw(string value)
        {
            Value = value;
        }

        public Raw() { }

        void ISqlElement.Render(RenderContext context)
        {
            if (Value != null)
            {
                context.Write(Value);
            }
        }
    }
}
