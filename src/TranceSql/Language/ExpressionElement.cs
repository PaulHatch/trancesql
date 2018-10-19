using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{

    /// <summary>
    /// Base class for SQL elements which supports expression operators.
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
            => new BinaryExpression(left, ArithmeticOperator.Add, right);

        /// <summary>
        /// Creates a binary expression from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new binary expression.
        /// </returns>
        public static BinaryExpression operator -(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Subtract, right);

        /// <summary>
        /// Creates a binary expression from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new binary expression.
        /// </returns>
        public static BinaryExpression operator *(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Multiply, right);

        /// <summary>
        /// Creates a binary expression from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new binary expression.
        /// </returns>
        public static BinaryExpression operator /(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Divide, right);

        /// <summary>
        /// Creates a binary expression from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new binary expression.
        /// </returns>
        public static BinaryExpression operator %(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Modulo, right);

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, left, right);

        /// <summary>
        /// Creates a condition from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition expression.
        /// </returns>
        public static Condition operator <(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThan, left, right);

        /// <summary>
        /// Creates a condition from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition expression.
        /// </returns>
        public static Condition operator >=(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, left, right);

        /// <summary>
        /// Creates a condition from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition expression.
        /// </returns>
        public static Condition operator <=(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, left, right);

        /// <summary>
        /// Creates a condition from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition expression.
        /// </returns>
        public static Condition operator ==(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.Equal, left, right);

        /// <summary>
        /// Creates a condition from two expression elements.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition expression.
        /// </returns>
        public static Condition operator !=(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, left, right);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="context">The render context.</param>
        public void Render(RenderContext context) => throw new NotImplementedException();
    }
}
