using System;

namespace TranceSql;

/// <summary>
/// Represents a where or filter clause in a SQL statement.
/// </summary>
public class FilterClause
{
    internal FilterClause(ConditionBase condition) => Value = condition;

    /// <summary>
    /// Gets the condition value for this instance.
    /// </summary>
    public ConditionBase Value { get; }

    /// <summary>
    /// Performs an implicit conversion from a condition to a filter clause.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator FilterClause(ConditionBase value)
        => new(value);

    /// <summary>Implements the AND operator.</summary>
    /// <param name="clause">The clause.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>The result of the operator.</returns>
    public static FilterClause operator &(FilterClause? clause, Condition condition)
    {
        return (clause?.Value) switch
        {
            null => new FilterClause(condition),
            Condition clauseCondition => clauseCondition & condition,
            ConditionPair clauseConditionPair => clauseConditionPair & condition,
            _ => throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported"),
        };
    }

    /// <summary>Implements the AND operator.</summary>
    /// <param name="clause">The clause.</param>
    /// <param name="conditionPair">The condition.</param>
    /// <returns>The result of the operator.</returns>
    public static FilterClause operator &(FilterClause? clause, ConditionPair conditionPair)
    {
        return (clause?.Value) switch
        {
            null => new FilterClause(conditionPair),
            Condition clauseCondition => clauseCondition & conditionPair,
            ConditionPair clauseConditionPair => clauseConditionPair & conditionPair,
            _ => throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported"),
        };
    }

    /// <summary>Implements the OR operator.</summary>
    /// <param name="clause">The clause.</param>
    /// <param name="condition">The condition pair.</param>
    /// <returns>The result of the operator.</returns>
    public static FilterClause operator |(FilterClause? clause, Condition condition)
    {
        return (clause?.Value) switch
        {
            null => new FilterClause(condition),
            Condition clauseCondition => clauseCondition | condition,
            ConditionPair clauseConditionPair => clauseConditionPair | condition,
            _ => throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported"),
        };
    }

    /// <summary>Implements the OR operator.</summary>
    /// <param name="clause">The clause.</param>
    /// <param name="conditionPair">The condition pair.</param>
    /// <returns>The result of the operator.</returns>
    public static FilterClause operator |(FilterClause? clause, ConditionPair conditionPair)
    {
        return (clause?.Value) switch
        {
            null => new FilterClause(conditionPair),
            Condition clauseCondition => clauseCondition | conditionPair,
            ConditionPair clauseConditionPair => clauseConditionPair | conditionPair,
            _ => throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported"),
        };
    }
}