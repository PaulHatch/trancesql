using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Represents a Postgres JSON operator or expression.
    /// </summary>
    /// <seealso cref="TranceSql.ISqlElement" />
    public class JsonExpression : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Gets the left element of this expression.
        /// </summary>
        public ISqlElement Left { get; }

        /// <summary>
        /// Gets the operator for this expression.
        /// </summary>
        public JsonOperator Symbol { get; }

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
        public JsonExpression(ExpressionElement left, JsonOperator symbol, ExpressionElement right)
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
                    case JsonOperator.Get:
                        context.Write("->");
                        break;
                    case JsonOperator.GetAsText:
                        context.Write("->>");
                        break;
                    case JsonOperator.GetByPath:
                        context.Write("#>");
                        break;
                    case JsonOperator.GetByPathAsText:
                        context.Write("#>>");
                        break;
                    case JsonOperator.LeftContainsRight:
                        context.Write("@>");
                        break;
                    case JsonOperator.RightContainsLeft:
                        context.Write("<@");
                        break;
                    case JsonOperator.Contains:
                        context.Write("?");
                        break;
                    case JsonOperator.ContainsAny:
                        context.Write("?|");
                        break;
                    case JsonOperator.Concat:
                        context.Write("||");
                        break;
                    case JsonOperator.Delete:
                        context.Write("-");
                        break;
                    case JsonOperator.DeletePath:
                        context.Write("#-");
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
