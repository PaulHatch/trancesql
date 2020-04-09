using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a condition, for example in a where clause.
    /// </summary>
    public class Condition : ConditionBase, ISqlElement
    {
        /// <summary>
        /// Initializes a new condition instance.
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public Condition(
            OperationType operationType,
            ISqlElement left,
            ISqlElement right)
        {
            OperationType = operationType;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Gets the type of the operation.
        /// </summary>
        public OperationType OperationType { get; }

        /// <summary>
        /// Gets the left condition element.
        /// </summary>
        public ISqlElement Left { get; }

        /// <summary>
        /// Gets the right condition element.
        /// </summary>
        public ISqlElement Right { get; }

        // Both sides explicitly provided

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Equal(ISqlElement left, ISqlElement right)
            => new Condition(OperationType.Equal, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(OperationType.NotEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(OperationType.GreaterThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(OperationType.GreaterThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(OperationType.LessThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(OperationType.LessThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(ISqlElement element)
            => new Condition(OperationType.IsNull, element, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(OperationType.IsNotNull, element, null);


        // Automatic column + value

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Equal(string column, object value)
            => new Condition(OperationType.Equal, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, object value)
            => new Condition(OperationType.NotEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, object value)
            => new Condition(OperationType.GreaterThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(OperationType.GreaterThanOrEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, object value)
            => new Condition(OperationType.LessThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(OperationType.LessThanOrEqual, new Column(column), new Value(value));

        // Automatic column w/table + value

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Equal(string table, string column, object value)
            => new Condition(OperationType.Equal, new Column(table, column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string table, string column, object value)
            => new Condition(OperationType.NotEqual, new Column(table, column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string table, string column, object value)
            => new Condition(OperationType.GreaterThan, new Column(table, column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string table, string column, object value)
            => new Condition(OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string table, string column, object value)
            => new Condition(OperationType.LessThan, new Column(table, column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string table, string column, object value)
            => new Condition(OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

        // Automatic column + explicit parameter

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Equal(string column, ISqlElement value)
            => new Condition(OperationType.Equal, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(OperationType.NotEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(OperationType.GreaterThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(OperationType.GreaterThanOrEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(OperationType.LessThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(OperationType.LessThanOrEqual, new Column(column), value);

        // Automatic column w/table + explicit parameter

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Equal(string table, string column, ISqlElement value)
            => new Condition(OperationType.Equal, new Column(table, column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string table, string column, ISqlElement value)
            => new Condition(OperationType.NotEqual, new Column(table, column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string table, string column, ISqlElement value)
            => new Condition(OperationType.GreaterThan, new Column(table, column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(OperationType.GreaterThanOrEqual, new Column(table, column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string table, string column, ISqlElement value)
            => new Condition(OperationType.LessThan, new Column(table, column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string table, string column, ISqlElement value)
            => new Condition(OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Exists(Select query)
            => new Condition(OperationType.Exists, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotExists(Select query)
            => new Condition(OperationType.NotExists, query, null);

        // in and not in query or values list

        #region In / Not In

        // With ISqlElement as column

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column for the condition.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(ISqlElement column, Select query)
            => new Condition(OperationType.In, column, query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column for the condition.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(ISqlElement column, Select query)
            => new Condition(OperationType.NotIn, column, query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column for the condition.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(ISqlElement column, Values values)
            => new Condition(OperationType.In, column, values);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column for the condition.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(ISqlElement column, params object[] values)
            => new Condition(OperationType.In, column, new Values(values));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column for the condition.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(ISqlElement column, params object[] values)
            => new Condition(OperationType.NotIn, column, new Value(values));

        // With multiple column

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="columns">The column for the condition.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(IEnumerable<Column> columns, Select query)
            => new Condition(OperationType.In, new Columns(columns), query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="columns">The column for the condition.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(IEnumerable<Column> columns, Select query)
            => new Condition(OperationType.NotIn, new Columns(columns), query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="columns">The column for the condition.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(IEnumerable<Column> columns, Values values)
            => new Condition(OperationType.In, new Columns(columns), values);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="columns">The column for the condition.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(IEnumerable<Column> columns, params object[] values)
            => new Condition(OperationType.In, new Columns(columns), new Values(values));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="columns">The column for the condition.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(IEnumerable<Column> columns, params object[] values)
            => new Condition(OperationType.NotIn, new Columns(columns), new Value(values));

        // With string column name

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(string column, Select query)
            => new Condition(OperationType.In, new Column(column), query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(string column, Select query)
            => new Condition(OperationType.NotIn, new Column(column), query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(string column, Values values)
            => new Condition(OperationType.In, new Column(column), values);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(string column, params object[] values)
            => new Condition(OperationType.In, new Column(column), new Values(values));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(string column, params object[] values)
            => new Condition(OperationType.NotIn, new Column(column), new Value(values));

        // With string column and table name

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(string table, string column, Select query)
            => new Condition(OperationType.In, new Column(table, column), query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(string table, string column, Select query)
            => new Condition(OperationType.NotIn, new Column(table, column), query);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(string table, string column, Values values)
            => new Condition(OperationType.In, new Column(table, column), values);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(string table, string column, params object[] values)
            => new Condition(OperationType.In, new Column(table, column), new Values(values));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(string table, string column, params object[] values)
            => new Condition(OperationType.NotIn, new Column(table, column), new Value(values));

        #endregion

        #region Is Null / Is Not Null

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string column)
            => new Condition(OperationType.IsNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string column)
            => new Condition(OperationType.IsNotNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string table, string column)
            => new Condition(OperationType.IsNull, new Column(table, column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string table, string column)
            => new Condition(OperationType.IsNotNull, new Column(table, column), null);

        #endregion

        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.Nested))
            {
                if (Left is null || Right is null)
                {
                    // A value is null, get the non-null value
                    var value = Left ?? Right;

                    if (value is null)
                    {
                        throw new InvalidCommandException("Both sides of a condition operation cannot be null.");
                    }

                    // Note that we are supporting binary operators Equal and NotEqual below to allow
                    // automatic conversion of null to IS NULL or IS NOT NULL clauses.

                    switch (OperationType)
                    {
                        case OperationType.Equal:
                        case OperationType.IsNull:
                            context.Render(value);
                            context.Write(" IS NULL");
                            return;

                        case OperationType.NotEqual:
                        case OperationType.IsNotNull:
                            context.Render(value);
                            context.Write(" IS NOT NULL");
                            return;

                        case OperationType.Exists:
                            context.Write("EXISTS ");
                            context.Render(value);
                            return;

                        case OperationType.NotExists:
                            context.Write("NOT EXISTS ");
                            context.Render(value);
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
                            context.Write(" IN ");
                            break;
                        case OperationType.NotIn:
                            context.Write(" NOT IN ");
                            break;
                        case OperationType.Exists:
                        case OperationType.NotExists:
                        case OperationType.IsNull:
                        case OperationType.IsNotNull:
                            throw new InvalidCommandException($"The unary operator '{OperationType}' does not support multiple arguments.");
                        default:
                            throw new InvalidCommandException($"Unrecognized operation '{OperationType}'.");
                    }
                    context.Render(Right);
                }
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
