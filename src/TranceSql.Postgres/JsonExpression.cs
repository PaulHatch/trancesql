using System;

namespace TranceSql.Postgres;

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
    public JsonExpressionOperator Symbol { get; }

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
    public JsonExpression(ExpressionElement left, JsonExpressionOperator symbol, ExpressionElement right)
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
                case JsonExpressionOperator.Get:
                    context.Write("->");
                    break;
                case JsonExpressionOperator.GetAsText:
                    context.Write("->>");
                    break;
                case JsonExpressionOperator.GetByPath:
                    context.Write("#>");
                    break;
                case JsonExpressionOperator.GetByPathAsText:
                    context.Write("#>>");
                    break;
                case JsonExpressionOperator.Concat:
                    context.Write("||");
                    break;
                case JsonExpressionOperator.Delete:
                    context.Write("-");
                    break;
                case JsonExpressionOperator.DeletePath:
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