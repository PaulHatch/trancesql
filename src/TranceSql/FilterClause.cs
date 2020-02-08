using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
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
        public static implicit operator FilterClause(Condition value)
            => new FilterClause(value);

        /// <summary>
        /// Performs an implicit conversion from a condition pair to a filter clause.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator FilterClause(ConditionPair value)
            => new FilterClause(value);

        /// <summary>Implements the AND operator.</summary>
        /// <param name="clause">The clause.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The result of the operator.</returns>
        public static FilterClause operator &(FilterClause clause, Condition condition)
        {
            switch (clause?.Value)
            {
                case null: return new FilterClause(condition);
                case Condition clauseCondition: return clauseCondition & condition;
                case ConditionPair clauseConditionPair: return clauseConditionPair & condition;
                default: throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported");
            }
        }

        /// <summary>Implements the AND operator.</summary>
        /// <param name="clause">The clause.</param>
        /// <param name="conditionPair">The condition.</param>
        /// <returns>The result of the operator.</returns>
        public static FilterClause operator &(FilterClause clause, ConditionPair conditionPair)
        {
            switch (clause?.Value)
            {
                case null: return new FilterClause(conditionPair);
                case Condition clauseCondition: return clauseCondition & conditionPair;
                case ConditionPair clauseConditionPair: return clauseConditionPair & conditionPair;
                default: throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported");
            }
        }

        /// <summary>Implements the OR operator.</summary>
        /// <param name="clause">The clause.</param>
        /// <param name="condition">The condition pair.</param>
        /// <returns>The result of the operator.</returns>
        public static FilterClause operator |(FilterClause clause, Condition condition)
        {
            switch (clause?.Value)
            {
                case null: return new FilterClause(condition);
                case Condition clauseCondition: return clauseCondition | condition;
                case ConditionPair clauseConditionPair: return clauseConditionPair | condition;
                default: throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported");
            }
        }

        /// <summary>Implements the OR operator.</summary>
        /// <param name="clause">The clause.</param>
        /// <param name="conditionPair">The condition pair.</param>
        /// <returns>The result of the operator.</returns>
        public static FilterClause operator |(FilterClause clause, ConditionPair conditionPair)
        {
            switch (clause?.Value)
            {
                case null: return new FilterClause(conditionPair);
                case Condition clauseCondition: return clauseCondition | conditionPair;
                case ConditionPair clauseConditionPair: return clauseConditionPair | conditionPair;
                default: throw new InvalidOperationException($"Operations on {clause.Value.GetType().Name} type filter clauses are not supported");
            }
        }
    }
}
