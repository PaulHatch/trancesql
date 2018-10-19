﻿using System.Collections.Generic;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a collection of assignment operations. This class supports implicit
    /// casting from assignment operators, add well as collection initialization of
    /// column to value and column to element assignment operations. See documentation
    /// of the <see cref="Update"/> command for usage examples.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{TranceSql.Language.Assignment}" />
    public class AssignmentCollection : List<Assignment>
    {
        /// <summary>
        /// Adds a new assignment of the specified column to the specified value. This
        /// value will automatically be parameterized and passed in to the final command.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value to assign.</param>
        public void Add(string column, object value)
            => Add(new Assignment(new Column(column), new Value(value)));

        /// <summary>
        /// Adds a new assignment of the specified column to the specified value. This
        /// value will automatically be parameterized and passed in to the final command.
        /// </summary>
        /// <param name="table">The column's table's name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value to assign.</param>
        public void Add(string table, string column, object value)
            => Add(new Assignment(new Column(table, column), new Value(value)));

        /// <summary>
        /// Adds a new assignment of the specified column to the specified value.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value to assign.</param>
        public void Add(string column, ISqlElement value)
            => Add(new Assignment(new Column(column), value));

        /// <summary>
        /// Adds a new assignment of the specified column to the specified value.
        /// </summary>
        /// <param name="table">The column's table's name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value to assign.</param>
        public void Add(string table, string column, ISqlElement value) 
            => Add(new Assignment(new Column(table, column), value));

        /// <summary>
        /// Adds a new assignment of the specified column to the specified value.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <param name="value">The value to assign.</param>
        public void Add(ISqlElement column, ISqlElement value)
            => Add(new Assignment(column, value));

        /// <summary>
        /// Performs an implicit conversion from <see cref="Assignment"/> to <see cref="AssignmentCollection"/>.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AssignmentCollection(Assignment assignment)
            => new AssignmentCollection {  assignment };
    }
}