using System;

namespace TranceSql;

// Suppress warning here since the == and != operators are used for generating conditions,
// and not as actual equality checks.
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()


/// <summary>
/// Base class for SQL elements which supports expression operators. Derived
/// classes must also implement <see cref="ISqlElement"/>.
/// </summary>
public abstract class ExpressionElement : ISqlElement
{
    // Arithmetic

    /// <summary>
    /// Creates a binary expression from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator +(ExpressionElement left, ExpressionElement right)
        => new(left, ArithmeticOperator.Add, right);

    /// <summary>
    /// Creates a binary expression from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator -(ExpressionElement left, ExpressionElement right)
        => new(left, ArithmeticOperator.Subtract, right);

    /// <summary>
    /// Creates a binary expression from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator *(ExpressionElement left, ExpressionElement right)
        => new(left, ArithmeticOperator.Multiply, right);

    /// <summary>
    /// Creates a binary expression from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator /(ExpressionElement left, ExpressionElement right)
        => new(left, ArithmeticOperator.Divide, right);

    /// <summary>
    /// Creates a binary expression from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator %(ExpressionElement left, ExpressionElement right)
        => new(left, ArithmeticOperator.Modulo, right);

    /// <summary>
    /// Creates a bit shift expression from an expression element.
    /// </summary>
    /// <param name="element">The element to be shifted.</param>
    /// <param name="places">The number of places to shift.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator <<(ExpressionElement element, int places)
        => new(element, ArithmeticOperator.BitShiftLeft, new Constant(places));

    /// <summary>
    /// Creates a bit shift expression from an expression element.
    /// </summary>
    /// <param name="element">The element to be shifted.</param>
    /// <param name="places">The number of places to shift.</param>
    /// <returns>
    /// A new binary expression.
    /// </returns>
    public static BinaryExpression operator >>(ExpressionElement element, int places)
        => new(element, ArithmeticOperator.BitShiftRight, new Constant(places));

    // Comparison

    /// <summary>
    /// Creates a condition from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new condition expression.
    /// </returns>
    public static Condition operator >(ExpressionElement left, ExpressionElement right)
        => new(OperationType.GreaterThan, left, right);

    /// <summary>
    /// Creates a condition from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new condition expression.
    /// </returns>
    public static Condition operator <(ExpressionElement left, ExpressionElement right)
        => new(OperationType.LessThan, left, right);

    /// <summary>
    /// Creates a condition from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new condition expression.
    /// </returns>
    public static Condition operator >=(ExpressionElement left, ExpressionElement right)
        => new(OperationType.GreaterThanOrEqual, left, right);

    /// <summary>
    /// Creates a condition from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new condition expression.
    /// </returns>
    public static Condition operator <=(ExpressionElement left, ExpressionElement right)
        => new(OperationType.LessThanOrEqual, left, right);

    /// <summary>
    /// Creates a condition from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new condition expression.
    /// </returns>
    public static Condition operator ==(ExpressionElement left, ExpressionElement right)
        => new(OperationType.Equal, left, right);

    /// <summary>
    /// Creates a condition from two expression elements.
    /// </summary>
    /// <param name="left">The left element.</param>
    /// <param name="right">The right element.</param>
    /// <returns>
    /// A new condition expression.
    /// </returns>
    public static Condition operator !=(ExpressionElement left, ExpressionElement right)
        => new(OperationType.NotEqual, left, right);

    /// <summary>
    /// Not implemented. This is included as an implementation rather than
    /// an abstract method so that derived classes do not expose the render
    /// method directly and clutter up the API.
    /// </summary>
    /// <param name="context">The render context.</param>
    void ISqlElement.Render(RenderContext context) => throw new NotImplementedException();
}

#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)