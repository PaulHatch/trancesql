using System;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql
{
    /// <summary>
    /// Represents a collection of condition clauses.
    /// </summary>
    public class ConditionCollection : List<ICondition>, ICondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        public ConditionCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection" /> class.
        /// </summary>
        /// <param name="nested">
        /// True if this collection is nested (wrapped in parentheses.
        /// </param>
        public ConditionCollection(bool nested)
        {
            IsNested = nested;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        /// <param name="type">
        /// The operator used to combine this collection with previous conditions.
        /// </param>
        public ConditionCollection(BooleanOperator type)
        {
            BooleanOperator = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        /// <param name="type">
        /// The operator used to combine this collection with previous conditions.
        /// </param>
        /// <param name="nested">True if this collection is nested (wrapped in parentheses.</param>
        public ConditionCollection(BooleanOperator type, bool nested)
        {
            BooleanOperator = type;
        }


        /// <summary>
        /// Adds the specified collection of conditions.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void Add(IEnumerable<ICondition> collection)
        {
            if (collection is ICondition condition)
            {
                // Ensure that nested condition collections are added as a unit, this
                // means if we have a collection and add a collection with two elements
                // in it, we get a single nested set added to the parent.
                Add(condition);
            }
            else
            {
                AddRange(collection);
            }
        }

        /// <summary>
        /// Gets or sets the boolean operator used to compare this collection against
        /// the left side condition if this collection is used within another condition collection.
        /// </summary>
        public BooleanOperator BooleanOperator { get; set; }

        void ISqlElement.Render(RenderContext context) => RenderCollection(context);

        /// <summary>
        /// Renders the where collection, adding AND and OR statements, to the specified .
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <exception cref="System.InvalidOperationException">Unknown operator value</exception>
        internal void RenderCollection(RenderContext context)
        {
            RenderCondition(context, this.First());
            foreach (var item in this.Skip(1))
            {
                switch (item.BooleanOperator)
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

                RenderCondition(context, item);
            }
        }

        private static void RenderCondition(RenderContext context, ICondition item)
        {
            if (item is ConditionCollection collection && collection.IsNested)
            {
                context.Write('(');
                context.Render(item);
                context.Write(')');
            }
            else
            {
                context.Render(item);
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Condition"/> to <see cref="ConditionCollection"/>.
        /// </summary>
        /// <param name="condition">The where clause.</param>
        /// <returns>
        /// A new collection containing the specified clause.
        /// </returns>
        public static implicit operator ConditionCollection(Condition condition) => new ConditionCollection { condition };


        /// <summary>
        /// Gets or sets a value indicating whether this instance is a nested collection.
        /// </summary>
        public bool IsNested { get; set; }

        private static ConditionCollection Combine(ICondition left, ICondition right, BooleanOperator type)
        {
            right.BooleanOperator = type;
            var leftCollection = left as ConditionCollection;
            var rightCollection = right as ConditionCollection;

            if (leftCollection?.IsNested == true || rightCollection?.IsNested == true)
            {
                return new ConditionCollection { left, right };
            }

            if (leftCollection != null)
            {
                leftCollection.Add(rightCollection ?? right);
                return leftCollection;
            }

            if (rightCollection != null)
            {
                // we already know that left is not a ConditionCollection here...
                rightCollection.Insert(0, left);
                return rightCollection;
            }

            throw new ArgumentException("Either left or right condition must be a collection");
        }

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
        public static ConditionCollection operator &(ConditionCollection left, Condition right)
            => Combine(left, right, BooleanOperator.And);

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionCollection operator &(ConditionCollection left, ConditionCollection right)
            => Combine(left, right, BooleanOperator.And);

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionCollection operator &(Condition left, ConditionCollection right)
            => Combine(left, right, BooleanOperator.And);

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionCollection operator |(ConditionCollection left, Condition right)
            => Combine(left, right, BooleanOperator.Or);

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionCollection operator |(ConditionCollection left, ConditionCollection right)
            => Combine(left, right, BooleanOperator.Or);

        /// <summary>
        /// Creates a new operation
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ConditionCollection operator |(Condition left, ConditionCollection right)
            => Combine(left, right, BooleanOperator.Or);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}