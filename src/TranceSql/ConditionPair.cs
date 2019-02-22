using System;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql
{
    /// <summary>
    /// Represents boolean pair of conditions combined with AND or OR.
    /// </summary>
    public class ConditionPair : ICondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionPair" /> class.
        /// </summary>
        /// <param name="type">The operator used to combine this collection with previous conditions.</param>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <param name="nested">True if this collection is nested (wrapped in parentheses.</param>
        public ConditionPair(BooleanOperator type, ICondition left, ICondition right)
            : this(type, left, right, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionPair" /> class.
        /// </summary>
        /// <param name="type">The operator used to combine this collection with previous conditions.</param>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <param name="nested">True if this collection is nested (wrapped in parentheses.</param>
        public ConditionPair(BooleanOperator type, ICondition left, ICondition right, bool nested)
        {
            Left = left;
            Right = right;
            IsNested = nested;
            BooleanOperator = type;
        }

        /// <summary>
        /// Gets or sets the boolean operator used to compare the conditions in this pair.
        /// </summary>
        public BooleanOperator BooleanOperator { get; set; }

        /// <summary>
        /// Creates an AND condition pair.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>A new condition pair.</returns>
        public static ConditionPair And(ICondition left, ICondition right) => new ConditionPair(BooleanOperator.And, left, right);
        
        /// <summary>
        /// Creates an OR condition pair.
        /// </summary>
        /// <param name="left">The left condition.</param>
        /// <param name="right">The right condition.</param>
        /// <returns>A new condition pair.</returns>
        public static ConditionPair Or(ICondition left, ICondition right) => new ConditionPair(BooleanOperator.Or, left, right);


        /// <summary>
        /// Renders the where collection, adding AND and OR statements, to the specified .
        /// </summary>
        /// <param name="context">The rendering context.</param>
        void ISqlElement.Render(RenderContext context)
        {
            if (IsNested)
            {
                context.Write('(');
            }

            context.Render(Left ?? throw new InvalidCommandException("Left condition missing"));
            switch (BooleanOperator)
            {
                case BooleanOperator.And:
                    context.Write(" AND ");
                    break;
                case BooleanOperator.Or:
                    context.Write(" OR ");
                    break;
                default:
                    throw new InvalidOperationException("Unknown operator value");
            }
            context.Render(Right ?? throw new InvalidCommandException("Right condition missing"));

            if (IsNested)
            {
                context.Write(')');
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a nested collection.
        /// </summary>
        public bool IsNested { get; set; }

        /// <summary>
        /// Gets the left condition clause.
        /// </summary>
        public ICondition Left { get; }

        /// <summary>
        /// Gets the left condition clause.
        /// </summary>
        public ICondition Right { get; }

        // These bool operators cover combining condition collections and mixed condition
        // collections and conditions. The condition class contains additional bool operators
        // for combining only conditions.

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionPair operator &(ConditionPair left, Condition right)
        {
            // Since AND operations come before OR, if the left pair is
            // an OR type it must have executed out of order
            if (left.BooleanOperator == BooleanOperator.Or)
            {
                left.IsNested = true;
            }

            return And(left, right);
        }

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionPair operator &(ConditionPair left, ConditionPair right)
        {
            // Since AND operations come before OR, if the left pair is
            // an OR type it must have executed out of order
            if (left.BooleanOperator == BooleanOperator.Or)
            {
                left.IsNested = true;
            }
            // Since AND operations come before OR and are executed left to
            // right, any right pair must have executed out of order
            right.IsNested = true;

            return And(left, right);
        }

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionPair operator &(Condition left, ConditionPair right)
        {
            // Since AND operations come before OR and are executed left to
            // right, any right pair must have executed out of order
            right.IsNested = true;

            return And(left, right);
        }

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionPair operator |(ConditionPair left, Condition right)
        {
            return Or(left, right);
        }

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionPair operator |(ConditionPair left, ConditionPair right)
        {
            // Since operations are right to left, if the right operation is
            // a pair it must have executed out of order
            if (right.BooleanOperator == BooleanOperator.Or)
            {
                right.IsNested = true;
            }
            return Or(left, right);
        }

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionPair operator |(Condition left, ConditionPair right)
        {
            // Since operations are right to left, if the right operation is
            // a pair it must have executed out of order
            if (right.BooleanOperator == BooleanOperator.Or)
            {
                right.IsNested = true;
            }
            return Or(left, right);
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