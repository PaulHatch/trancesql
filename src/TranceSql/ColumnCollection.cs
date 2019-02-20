using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a collection of columns or other selectable elements. This class supports
    /// implicit casting from <see cref="String"/>, <see cref="Alias"/>,
    /// and any <see cref="ExpressionElement"/> class (which includes <see cref="Column"/>)
    /// add well as collection initialization of columns from string values. See documentation
    /// of the <see cref="Select"/> command for usage examples.
    /// </summary>
    public class ColumnCollection : List<ISqlElement>
    {
        /// <summary>
        /// Adds a new column with the specified name.
        /// </summary>
        /// <param name="name">The column name.</param>
        public void Add(string name)
        {
            Add(new Column(name));
        }

        /// <summary>
        /// Adds a new column with the specified name.
        /// </summary>
        /// <param name="table">The column's table's name.</param>
        /// <param name="name">The column name.</param>
        public void Add(string table, string name)
        {
            Add(new Column(table, name));
        }

        /// <summary>
        /// Adds a new column with the specified name.
        /// </summary>
        /// <param name="schema">The column's table's schema's name.</param>
        /// <param name="table">The column's table's name.</param>
        /// <param name="name">The column name.</param>
        public void Add(string schema, string table, string name)
        {
            Add(new Column(schema, table, name));
        }

        /// <summary>
        /// Adds a range of column elements.
        /// </summary>
        /// <param name="elements">The elements to add.</param>
        public void Add(IEnumerable<ISqlElement> elements)
        {
            AddRange(elements);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ColumnCollection"/>.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColumnCollection(string column)
            => new ColumnCollection { column };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Alias"/> to <see cref="ColumnCollection"/>.
        /// </summary>
        /// <param name="aliasedElement">The aliased element.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColumnCollection(Alias aliasedElement)
            => new ColumnCollection { aliasedElement };

        /// <summary>
        /// Performs an implicit conversion from <see cref="ExpressionElement"/> to <see cref="ColumnCollection"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColumnCollection(ExpressionElement element)
            => new ColumnCollection { element };
    }
}
