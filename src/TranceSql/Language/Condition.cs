using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public static class And
    {
        public static Condition Condition(Condition condition)
        {
            condition.BooleanOperator = BooleanOperator.And;
            return condition;
        }

        public static ConditionCollection Nested(ICondition left, ICondition right)
            => new ConditionCollection(BooleanOperator.And, true) { left, right };

        public static ConditionCollection Nested(ICondition left, ICondition middle, ICondition right)
            => new ConditionCollection(BooleanOperator.And, true) { left, middle, right };

        public static ConditionCollection Nested(params ICondition[] conditions)
            => new ConditionCollection(BooleanOperator.And, true) { conditions };

        // Both sides explicitly provided

        public static Condition Equal(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.Equal, left, right);
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, left, right);
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, left, right);
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, left, right);
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThan, left, right);
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, left, right);

        public static Condition IsNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNull, element, null);
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, element, null);


        // Automatic column + value

        public static Condition Equal(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), new Value(value));
        public static Condition NotEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), new Value(value));
        public static Condition GreaterThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), new Value(value));
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), new Value(value));
        public static Condition LessThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), new Value(value));
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), new Value(value));

        // Automatic column w/table + value

        public static Condition Equal(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), new Value(value));
        public static Condition NotEqual(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), new Value(value));
        public static Condition GreaterThan(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), new Value(value));
        public static Condition GreaterThanOrEqual(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));
        public static Condition LessThan(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), new Value(value));
        public static Condition LessThanOrEqual(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

        // Automatic column + explicit parameter

        public static Condition Equal(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), value);
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), value);
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), value);
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), value);
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), value);
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), value);

        // Automatic column w/table + explicit parameter

        public static Condition Equal(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), value);
        public static Condition NotEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), value);
        public static Condition GreaterThan(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), value);
        public static Condition GreaterThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), value);
        public static Condition LessThan(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), value);
        public static Condition LessThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        public static Condition Exists(Select query)
            => new Condition(BooleanOperator.And, OperationType.Exists, query, null);

        public static Condition NotExists(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotExists, query, null);

        // in and not in query or values list

        public static Condition In(Select query)
            => new Condition(BooleanOperator.And, OperationType.In, query, null);
        public static Condition NotIn(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotIn, query, null);

        public static Condition In(Values values)
            => new Condition(BooleanOperator.And, OperationType.In, values, null);
        public static Condition NotIn(Values values)
            => new Condition(BooleanOperator.And, OperationType.NotIn, values, null);

        // Automatic null
        public static Condition IsNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(column), null);
        public static Condition IsNotNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(column), null);
        public static Condition IsNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(table, column), null);
        public static Condition IsNotNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(table, column), null);
    }

    public static class Or
    {
        public static Condition Condition(Condition condition)
        {
            condition.BooleanOperator = BooleanOperator.Or;
            return condition;
        }

        public static ConditionCollection Nested(ICondition left, ICondition right)
            => new ConditionCollection(BooleanOperator.Or, true) { left, right };

        public static ConditionCollection Nested(ICondition left, ICondition middle, ICondition right)
            => new ConditionCollection(BooleanOperator.Or, true) { left, middle, right };

        public static ConditionCollection Nested(params ICondition[] conditions)
            => new ConditionCollection(BooleanOperator.Or, true) { conditions };

        // Both sides explicitly provided

        public static Condition Equal(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.Equal, left, right);
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, left, right);
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, left, right);
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, left, right);
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, left, right);
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, left, right);

        public static Condition IsNull(ISqlElement element)
            => new Condition(BooleanOperator.Or, OperationType.IsNull, element, null);
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(BooleanOperator.Or, OperationType.IsNotNull, element, null);


        // Automatic column + value

        public static Condition Equal(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(column), new Value(value));
        public static Condition NotEqual(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(column), new Value(value));
        public static Condition GreaterThan(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(column), new Value(value));
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(column), new Value(value));
        public static Condition LessThan(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(column), new Value(value));
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(column), new Value(value));

        // Automatic column w/table + value

        public static Condition Equal(string table, string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(table, column), new Value(value));
        public static Condition NotEqual(string table, string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(table, column), new Value(value));
        public static Condition GreaterThan(string table, string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(table, column), new Value(value));
        public static Condition GreaterThanOrEqual(string table, string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));
        public static Condition LessThan(string table, string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(table, column), new Value(value));
        public static Condition LessThanOrEqual(string table, string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

        // Automatic column + explicit parameter

        public static Condition Equal(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(column), value);
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(column), value);
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(column), value);
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(column), value);
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(column), value);
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(column), value);

        // Automatic column w/table + explicit parameter

        public static Condition Equal(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(table, column), value);
        public static Condition NotEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(table, column), value);
        public static Condition GreaterThan(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(table, column), value);
        public static Condition GreaterThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(table, column), value);
        public static Condition LessThan(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(table, column), value);
        public static Condition LessThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        public static Condition Exists(Select query)
            => new Condition(BooleanOperator.Or, OperationType.Exists, query, null);

        public static Condition NotExists(Select query)
            => new Condition(BooleanOperator.Or, OperationType.NotExists, query, null);

        // in and not in query or values list

        public static Condition In(Select query)
            => new Condition(BooleanOperator.Or, OperationType.In, query, null);
        public static Condition NotIn(Select query)
            => new Condition(BooleanOperator.Or, OperationType.NotIn, query, null);

        public static Condition In(Values values)
            => new Condition(BooleanOperator.Or, OperationType.In, values, null);
        public static Condition NotIn(Values values)
            => new Condition(BooleanOperator.Or, OperationType.NotIn, values, null);

        // Automatic null
        public static Condition IsNull(string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNull, new Column(column), null);
        public static Condition IsNotNull(string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNotNull, new Column(column), null);
        public static Condition IsNull(string table, string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNull, new Column(table, column), null);
        public static Condition IsNotNull(string table, string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNotNull, new Column(table, column), null);

    }


    public class Condition : ICondition
    {
        public Condition(
            BooleanOperator combineType,
            OperationType operationType,
            ISqlElement left,
            ISqlElement right)
        {
            BooleanOperator = combineType;
            OperationType = operationType;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Gets or sets a value indicating how this clause is combined with the previous one.
        /// </summary>
        /// <remarks>
        /// Note that the combine operator ("AND" or "OR") itself is rendered externally by
        /// a <see cref="ConditionCollection"/>, not by the condition itself.
        /// </remarks>
        public BooleanOperator BooleanOperator { get; set; }

        public OperationType OperationType { get; }
        public ISqlElement Left { get; }
        public ISqlElement Right { get; }

        public static ConditionCollection Nested(ICondition left, ICondition right)
            => new ConditionCollection(true) { left, right };

        public static ConditionCollection Nested(ICondition left, ICondition middle, ICondition right)
            => new ConditionCollection(true) { left, middle, right };

        public static ConditionCollection Nested(params ICondition[] conditions)
            => new ConditionCollection(true) { conditions };

        // Both sides explicitly provided

        public static Condition Equal(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.Equal, left, right);
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, left, right);
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, left, right);
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, left, right);
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThan, left, right);
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, left, right);

        public static Condition IsNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNull, element, null);
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, element, null);


        // Automatic column + value

        public static Condition Equal(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), new Value(value));
        public static Condition NotEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), new Value(value));
        public static Condition GreaterThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), new Value(value));
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), new Value(value));
        public static Condition LessThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), new Value(value));
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), new Value(value));

        // Automatic column w/table + value

        public static Condition Equal(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), new Value(value));
        public static Condition NotEqual(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), new Value(value));
        public static Condition GreaterThan(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), new Value(value));
        public static Condition GreaterThanOrEqual(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));
        public static Condition LessThan(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), new Value(value));
        public static Condition LessThanOrEqual(string table, string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

        // Automatic column + explicit parameter

        public static Condition Equal(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), value);
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), value);
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), value);
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), value);
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), value);
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), value);

        // Automatic column w/table + explicit parameter

        public static Condition Equal(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), value);
        public static Condition NotEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), value);
        public static Condition GreaterThan(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), value);
        public static Condition GreaterThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), value);
        public static Condition LessThan(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), value);
        public static Condition LessThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        public static Condition Exists(Select query)
            => new Condition(BooleanOperator.And, OperationType.Exists, query, null);

        public static Condition NotExists(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotExists, query, null);

        // in and not in query or values list

        public static Condition In(Select query)
            => new Condition(BooleanOperator.And, OperationType.In, query, null);
        public static Condition NotIn(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotIn, query, null);

        public static Condition In(Values values)
            => new Condition(BooleanOperator.And, OperationType.In, values, null);
        public static Condition NotIn(Values values)
            => new Condition(BooleanOperator.And, OperationType.NotIn, values, null);

        // Automatic null
        public static Condition IsNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(column), null);
        public static Condition IsNotNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(column), null);
        public static Condition IsNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(table, column), null);
        public static Condition IsNotNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(table, column), null);

        public static ConditionCollection operator &(Condition left, Condition right)
        {
            right.BooleanOperator = BooleanOperator.And;
            return new ConditionCollection { left, right };
        }
        
        public static ConditionCollection operator |(Condition left, Condition right)
        {
            right.BooleanOperator = BooleanOperator.Or;
            return new ConditionCollection { left, right };
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (Left == null || Right is null)
            {
                // A value is null, get the non-null value
                var value = Left ?? Right;

                if (value is null)
                {
                    throw new InvalidCommandException("Both sides of a where clause cannot be null.");
                }


                switch (OperationType)
                {
                    case OperationType.Equal:
                    case OperationType.NotEqual:
                        context.Render(value);
                        context.Write(" IS NULL");
                        return;

                    case OperationType.Exists:
                        context.Write("EXISTS ");
                        context.Render(value);
                        return;

                    case OperationType.NotExists:
                        context.Write("NOT EXISTS ");
                        context.Render(value);
                        return;

                    case OperationType.IsNull:
                        context.Render(value);
                        context.Write(" IS NULL");
                        return;

                    case OperationType.IsNotNull:
                        context.Render(value);
                        context.Write(" IS NOT NULL");
                        return;


                    default:
                        throw new InvalidCommandException($"The binary operator '{OperationType}' requires two arguments.");
                }

            }
            else
            {
                context.Render(Left);
                switch (OperationType)
                {
                    case OperationType.Equal:
                        context.Write(" = ");
                        break;
                    case OperationType.NotEqual:
                        context.Write(" <> ");
                        break;
                    case OperationType.GreaterThan:
                        context.Write(" > ");
                        break;
                    case OperationType.GreaterThanOrEqual:
                        context.Write(" >= ");
                        break;
                    case OperationType.LessThan:
                        context.Write(" < ");
                        break;
                    case OperationType.LessThanOrEqual:
                        context.Write(" <= ");
                        break;
                    case OperationType.In:
                    case OperationType.Exists:
                    case OperationType.NotExists:
                    case OperationType.NotIn:
                        throw new InvalidCommandException($"The unary operator '{OperationType}' does not support multiple arguments.");
                    default:
                        throw new InvalidCommandException($"Unrecognized operation '{OperationType}'.");
                }
                context.Render(Right);
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}
