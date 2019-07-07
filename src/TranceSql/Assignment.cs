using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{

    /// <summary>
    /// SQL element to assign a target = value.
    /// </summary>
    public class Assignment : ISqlStatement
    {

        /// <summary>
        /// SQL element to assign a target = value.
        /// </summary>
        /// <param name="target">The target to update.</param>
        /// <param name="value">The source of the value to be assigned.</param>
        public Assignment(ISqlElement target, ISqlElement value)
        {
            Target = target;
            Value = value;
        }

        /// <summary>
        /// Gets the target element to update.
        /// </summary>
        public ISqlElement Target { get; }
        
        /// <summary>
        /// Gets the value to be assigned.
        /// </summary>
        public ISqlElement Value { get; }

        void ISqlElement.Render(RenderContext context)
        {
            if (context.Mode == RenderMode.Nested)
            {
                switch (Target)
                {
                    case Column column:
                        context.WriteIdentifier(column.Name);
                        break;
                    default:
                        context.Render(Target);
                        break;
                }
                context.Write(" = ");
                context.Render(Value);
            }
            else
            {
                using (context.EnterChildMode(RenderMode.Nested))
                {
                    context.Write("SET ");
                    context.Render(Target);

                    context.Write(" = ");
                    context.Render(Value);
                    context.Write(';');
                }
            }
        }
    }
}
