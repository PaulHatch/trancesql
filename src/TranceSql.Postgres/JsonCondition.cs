using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Represents a Postgres JSON operator or expression.
    /// </summary>
    /// <seealso cref="TranceSql.ISqlElement" />
    public class JsonCondition : ConditionBase, ISqlElement
    {
        /// <summary>
        /// Gets the left element of this expression.
        /// </summary>
        public ISqlElement Left { get; }

        /// <summary>
        /// Gets the operator for this expression.
        /// </summary>
        public JsonConditionOperator Symbol { get; }

        /// <summary>
        /// Gets the right element of this expression.
        /// </summary>
        public ISqlElement Right { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExpression"/> class.
        /// </summary>
        /// <param name="left">The left element for this expression. Must not be null.</param>
        /// <param name="symbol">The operator for this expression.</param>
        /// <param name="right">The right element for this expression. Must not be null.</param>
        public JsonCondition(ExpressionElement left, JsonConditionOperator symbol, ExpressionElement right)
        {
            Left = left as ISqlElement ?? throw new ArgumentNullException(nameof(left));
            Symbol = symbol;
            Right = right as ISqlElement ?? throw new ArgumentNullException(nameof(right));
        }

        /// <inheritdoc/>
        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.Nested))
            {
                context.Render(Left);
                switch (Symbol)
                {
                    case JsonConditionOperator.LeftContainsRight:
                        context.Write("@>");
                        break;
                    case JsonConditionOperator.RightContainsLeft:
                        context.Write("<@");
                        break;
                    case JsonConditionOperator.Contains:
                        context.Write("?");
                        break;
                    case JsonConditionOperator.ContainsAny:
                        context.Write("?|");
                        break;
                    default:
                        break;
                }
                context.Render(Right);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}
