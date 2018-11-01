using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Utility for adding conditions which are combined with previous using AND.
    /// </summary>
    public static class And
    {
        /// <summary>
        /// Makes the specified expression an AND type and returns it.
        /// </summary>
        /// <param name="condition">The condition to modify.</param>
        /// <returns>An AND type condition.</returns>
        public static Condition Condition(Condition condition)
        {
            condition.BooleanOperator = BooleanOperator.And;
            return condition;
        }

        /// <summary>
        /// Creates a nested condition joined with AND.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>
        /// An AND type condition collection.
        /// </returns>
        public static ConditionCollection Nested(ICondition left, ICondition right)
            => new ConditionCollection(BooleanOperator.And, true) { left, right };

        /// <summary>
        /// Creates a nested condition joined with AND.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="middle">The middle condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>
        /// An AND type condition collection.
        /// </returns>
        public static ConditionCollection Nested(ICondition left, ICondition middle, ICondition right)
            => new ConditionCollection(BooleanOperator.And, true) { left, middle, right };

        /// <summary>
        /// Creates a nested condition joined with AND.
        /// </summary>
        /// <param name="conditions">The inner conditions.</param>
        /// <returns>
        /// An AND type condition collection.
        /// </returns>
        public static ConditionCollection Nested(params ICondition[] conditions)
            => new ConditionCollection(BooleanOperator.And, true) { conditions };

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
            => new Condition(BooleanOperator.And, OperationType.Equal, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNull, element, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, element, null);


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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), value);

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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), value);
        
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
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Exists(Select query)
            => new Condition(BooleanOperator.And, OperationType.Exists, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotExists(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotExists, query, null);

        // in and not in query or values list

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(Select query)
            => new Condition(BooleanOperator.And, OperationType.In, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotIn, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(Values values)
            => new Condition(BooleanOperator.And, OperationType.In, values, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(Values values)
            => new Condition(BooleanOperator.And, OperationType.NotIn, values, null);

        // Automatic null

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(table, column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(table, column), null);
    }

    /// <summary>
    /// Utility for adding conditions which are combined with previous using OR.
    /// </summary>
    public static class Or
    {
        /// <summary>
        /// Makes the specified expression an OR type and returns it.
        /// </summary>
        /// <param name="condition">The condition to modify.</param>
        /// <returns>An OR type condition.</returns>
        public static Condition Condition(Condition condition)
        {
            condition.BooleanOperator = BooleanOperator.Or;
            return condition;
        }

        /// <summary>
        /// Creates a nested condition joined with OR.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>
        /// An OR type condition collection.
        /// </returns>
        public static ConditionCollection Nested(ICondition left, ICondition right)
            => new ConditionCollection(BooleanOperator.Or, true) { left, right };

        /// <summary>
        /// Creates a nested condition joined with OR.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="middle">The middle condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>
        /// An OR type condition collection.
        /// </returns>
        public static ConditionCollection Nested(ICondition left, ICondition middle, ICondition right)
            => new ConditionCollection(BooleanOperator.Or, true) { left, middle, right };

        /// <summary>
        /// Creates a nested condition joined with OR.
        /// </summary>
        /// <param name="conditions">The inner conditions.</param>
        /// <returns>
        /// An OR type condition collection.
        /// </returns>
        public static ConditionCollection Nested(params ICondition[] conditions)
            => new ConditionCollection(BooleanOperator.Or, true) { conditions };

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
            => new Condition(BooleanOperator.Or, OperationType.Equal, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(ISqlElement element)
            => new Condition(BooleanOperator.Or, OperationType.IsNull, element, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(BooleanOperator.Or, OperationType.IsNotNull, element, null);


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
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(column), value);
        
        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(column), value);

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
            => new Condition(BooleanOperator.Or, OperationType.Equal, new Column(table, column), value);

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
            => new Condition(BooleanOperator.Or, OperationType.NotEqual, new Column(table, column), value);

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
            => new Condition(BooleanOperator.Or, OperationType.GreaterThan, new Column(table, column), value);

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
            => new Condition(BooleanOperator.Or, OperationType.GreaterThanOrEqual, new Column(table, column), value);

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
            => new Condition(BooleanOperator.Or, OperationType.LessThan, new Column(table, column), value);

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
            => new Condition(BooleanOperator.Or, OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Exists(Select query)
            => new Condition(BooleanOperator.Or, OperationType.Exists, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotExists(Select query)
            => new Condition(BooleanOperator.Or, OperationType.NotExists, query, null);

        // in and not in query or values list

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(Select query)
            => new Condition(BooleanOperator.Or, OperationType.In, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(Select query)
            => new Condition(BooleanOperator.Or, OperationType.NotIn, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(Values values)
            => new Condition(BooleanOperator.Or, OperationType.In, values, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(Values values)
            => new Condition(BooleanOperator.Or, OperationType.NotIn, values, null);

        // Automatic null

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNotNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string table, string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNull, new Column(table, column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string table, string column)
            => new Condition(BooleanOperator.Or, OperationType.IsNotNull, new Column(table, column), null);
    }

    /// <summary>
    /// Represents a condition, for example in a where clause.
    /// </summary>
    public class Condition : ICondition
    {
        /// <summary>
        /// Initializes a new condition instance.
        /// </summary>
        /// <param name="combineType">Type of the combine.</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
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

        /// <summary>
        /// Creates a nested condition. By default it is joined with AND.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>
        /// An condition collection.
        /// </returns>
        public static ConditionCollection Nested(ICondition left, ICondition right)
            => new ConditionCollection(true) { left, right };

        /// <summary>
        /// Creates a nested condition. By default it is joined with AND.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="middle">The middle condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>
        /// An condition collection.
        /// </returns>
        public static ConditionCollection Nested(ICondition left, ICondition middle, ICondition right)
            => new ConditionCollection(true) { left, middle, right };

        /// <summary>
        /// Creates a nested condition. By default it is joined with AND.
        /// </summary>
        /// <param name="conditions">The nested conditions.</param>
        /// <returns>
        /// An condition collection.
        /// </returns>
        public static ConditionCollection Nested(params ICondition[] conditions)
            => new ConditionCollection(true) { conditions };

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
            => new Condition(BooleanOperator.And, OperationType.Equal, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThan, left, right);
        
        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="left">The left element.</param>
        /// <param name="right">The right element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(ISqlElement left, ISqlElement right)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, left, right);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNull, element, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="element">The condition element.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(ISqlElement element)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, element, null);


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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), new Value(value));
        
        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), new Value(value));

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), new Value(value));
        
        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value. This value will be automatically parameterized.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, object value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), new Value(value));
        
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
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), new Value(value));

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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition GreaterThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThan(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(column), value);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition LessThanOrEqual(string column, ISqlElement value)
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(column), value);

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
            => new Condition(BooleanOperator.And, OperationType.Equal, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.NotEqual, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThan, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.GreaterThanOrEqual, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.LessThan, new Column(table, column), value);

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
            => new Condition(BooleanOperator.And, OperationType.LessThanOrEqual, new Column(table, column), value);

        // exists and not exists in query

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition Exists(Select query)
            => new Condition(BooleanOperator.And, OperationType.Exists, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotExists(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotExists, query, null);

        // in and not in query or values list

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(Select query)
            => new Condition(BooleanOperator.And, OperationType.In, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="query">The query for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(Select query)
            => new Condition(BooleanOperator.And, OperationType.NotIn, query, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition In(Values values)
            => new Condition(BooleanOperator.And, OperationType.In, values, null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="values">The values for the condition.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition NotIn(Values values)
            => new Condition(BooleanOperator.And, OperationType.NotIn, values, null);

        // Automatic null

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNull, new Column(table, column), null);

        /// <summary>
        /// Creates a new condition.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column name.</param>
        /// <returns>
        /// A new condition.
        /// </returns>
        public static Condition IsNotNull(string table, string column)
            => new Condition(BooleanOperator.And, OperationType.IsNotNull, new Column(table, column), null);

        /// <summary>
        /// Combines two conditions using AND.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// A new condition collection.
        /// </returns>
        public static ConditionCollection operator &(Condition left, Condition right)
        {
            right.BooleanOperator = BooleanOperator.And;
            return new ConditionCollection { left, right };
        }

        // Some of the bool operators are here, the reset are in ConditionCollection class

        /// <summary>
        /// Combines two conditions using OR.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// A new condition collection.
        /// </returns>
        public static ConditionCollection operator |(Condition left, Condition right)
        {
            right.BooleanOperator = BooleanOperator.Or;
            return new ConditionCollection { left, right };
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (Left is null || Right is null)
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}
