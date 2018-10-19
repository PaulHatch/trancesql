using System;

namespace TranceSql
{
    /// <summary>
    /// Represents an expression with a left and right element.
    /// </summary>
    public class BinaryExpression : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Gets the left element of this expression.
        /// </summary>
        public ISqlElement Left { get; }
        
        /// <summary>
        /// Gets the operator for this expression.
        /// </summary>
        public ArithmeticOperator Symbol { get; }

        /// <summary>
        /// Gets the right element of this expression.
        /// </summary>
        public ISqlElement Right { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="left">The left element for this expression. Must not be null.</param>
        /// <param name="symbol">The operator for this expression.</param>
        /// <param name="right">The right element for this expression. Must not be null.</param>
        public BinaryExpression(ExpressionElement left, ArithmeticOperator symbol, ExpressionElement right)
        {
            Left = left as ISqlElement ?? throw new ArgumentNullException(nameof(left));
            Symbol = symbol;
            Right = right as ISqlElement ?? throw new ArgumentNullException(nameof(right));
        }

        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.Nested))
            {
                context.Render(Left);
                switch (Symbol)
                {
                    case ArithmeticOperator.Add:
                        context.Write(" + ");
                        break;
                    case ArithmeticOperator.Subtract:
                        context.Write(" - ");
                        break;
                    case ArithmeticOperator.Multiply:
                        context.Write(" * ");
                        break;
                    case ArithmeticOperator.Divide:
                        context.Write(" / ");
                        break;
                    case ArithmeticOperator.Modulo:
                        context.Write(" % ");
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
