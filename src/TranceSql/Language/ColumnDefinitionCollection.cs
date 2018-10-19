using System.Collections;
using System.Collections.Generic;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a collection of column definitions. This class supports
    /// collection initialization of columns <see cref="string"/> and 
    /// <see cref="SqlType"/> combinations. See documentation of the 
    /// <see cref="CreateTable"/> command for usage examples.
    /// </summary>
    public class ColumnDefinitionCollection : List<ColumnDefinition>
    {
        /// <summary>
        /// Adds a column definition for the specified name and type.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="sqlType">The column type.</param>
        public void Add(string name, SqlType sqlType)
        {
            Add(new ColumnDefinition(name, sqlType, null));
        }

        /// <summary>
        /// Adds a column definition for the specified name and type with a
        /// constraint.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="sqlType">The column type.</param>
        /// <param name="constraint">A constraint for the column.</param>
        public void Add(string name, SqlType sqlType, IConstraint constraint)
        {
            Add(new ColumnDefinition(name, sqlType, new[] { constraint }));
        }

        /// <summary>
        /// Adds a column definition for the specified name and type with
        /// constraints.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="sqlType">The column type.</param>
        /// <param name="constraints">Constraints for the column.</param>
        public void Add(string name, SqlType sqlType, params IConstraint[] constraints)
        {
            Add(new ColumnDefinition(name, sqlType, constraints));
        }

        /// <summary>
        /// Adds the specified columns to this collection.
        /// </summary>
        /// <param name="columns">The columns to add.</param>
        public void Add(IEnumerable<ColumnDefinition> columns)
        {
            AddRange(columns);
        }
    }
}