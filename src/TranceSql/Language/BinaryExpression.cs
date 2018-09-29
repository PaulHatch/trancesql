using System;

namespace TranceSql.Language
{
    public class BinaryExpression : ExpressionElement, ISqlElement
    {
        public ISqlElement Left { get; }
        public ArithmeticOperator Symbol { get; }
        public ISqlElement Right { get; }

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

        public override string ToString() => this.RenderDebug();
    }
}
