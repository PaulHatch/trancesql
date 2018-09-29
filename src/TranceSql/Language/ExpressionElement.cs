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

        public static BinaryExpression operator +(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Add, right);

        public static BinaryExpression operator -(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Subtract, right);

        public static BinaryExpression operator *(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Multiply, right);

        public static BinaryExpression operator /(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Divide, right);

        public static BinaryExpression operator %(ExpressionElement left, ExpressionElement right)
            => new BinaryExpression(left, ArithmeticOperator.Modulo, right);

        // Comparison

        public static Condition operator >(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, left, right);

        public static Condition operator <(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThan, left, right);

        public static Condition operator >=(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, left, right);

        public static Condition operator <=(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, left, right);

        public static Condition operator ==(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.Equal, left, right);

        public static Condition operator !=(ExpressionElement left, ExpressionElement right)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, left, right);

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Render(RenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
