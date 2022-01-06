using System.Collections.Generic;

namespace TranceSql
{
    /// <summary>
    /// Represents a collection of join clause elements. This class supports implicit casting from 
    /// <see cref="Join"/>, as well as collection initialization from <see cref="JoinType"/>, 
    /// <see cref="string"/>, and <see cref="ConditionBase"/> combinations. See documentation of 
    /// the <see cref="Select"/> command for usage examples.
    /// </summary>
    public class JoinCollection : List<Join>
    {
        /// <summary>
        /// Adds the specified join to this collection.
        /// </summary>
        /// <param name="joinType">Type of the join to add.</param>
        /// <param name="table">The table name.</param>
        /// <param name="on">The join condition.</param>
        public void Add(JoinType joinType, string table, ConditionBase on)
        {
            Add(new Join(joinType, new Table(table), on));
        }

        /// <summary>
        /// Adds the specified join to this collection.
        /// </summary>
        /// <param name="joinType">Type of the join to add.</param>
        /// <param name="schema">The table's schema's name.</param>
        /// <param name="table">The table name.</param>
        /// <param name="on">The join condition.</param>
        public void Add(JoinType joinType, string schema, string table, ConditionBase on)
        {
            Add(new Join(joinType, new Table(table), on));
        }

        /// <summary>
        /// Adds the specified join to this collection.
        /// </summary>
        /// <param name="joinType">Type of the join to add.</param>
        /// <param name="table">The table name.</param>
        public void Add(JoinType joinType, string table)
        {
            Add(new Join(joinType, new Table(table), null));
        }

        /// <summary>
        /// Adds the specified join to this collection.
        /// </summary>
        /// <param name="joinType">Type of the join to add.</param>
        /// <param name="schema">The table's schema's name.</param>
        /// <param name="table">The table name.</param>
        public void Add(JoinType joinType, string schema, string table)
        {
            Add(new Join(joinType, new Table(table), null));
        }

        /// <summary>
        /// Adds the specified join to this collection.
        /// </summary>
        /// <param name="join">The join.</param>
        public void Add(Alias join)
        {
            if (join.Element is Join)
            {
                Add(join);
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Join"/> to <see cref="JoinCollection"/>.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JoinCollection(Join join)
            => new() { join };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Alias" /> to <see cref="JoinCollection" />.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JoinCollection(Alias join)
            => new() { join };
    }
}