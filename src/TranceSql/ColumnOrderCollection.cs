namespace TranceSql
{
    /// <summary>
    /// Represents a collection of column order statements. This class supports
    /// implicit conversion from <see cref="Column"/>, <see cref="Order"/>, and 
    /// <see cref="string"/> and collection initialization of columns using 
    /// <see cref="string"/> and <see cref="Direction"/> combinations, as well
    /// as all collection initialization methods inherited from 
    /// <see cref="SelectableCollection"/>. See documentation of the <see cref="Select"/>
    /// command for usage examples.
    /// </summary>
    public class ColumnOrderCollection : SelectableCollection
    {
        /// <summary>
        /// Adds a new ordering statement using the specified column.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <param name="direction">The order direction.</param>
        public void Add(string column, Direction direction)
            => Add(new Order(new Column(column), direction));

        /// <summary>
        /// Adds a new ordering statement using the specified column.
        /// </summary>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column to order by.</param>
        /// <param name="direction">The order direction.</param>
        public void Add(string table, string column, Direction direction)
            => Add(new Order(new Column(table, column), direction));

        /// <summary>
        /// Adds a new ordering statement using the specified column.
        /// </summary>
        /// <param name="schema">The column's table's schema.</param>
        /// <param name="table">The column's table name.</param>
        /// <param name="column">The column to order by.</param>
        /// <param name="direction">The order direction.</param>
        public void Add(string schema, string table, string column, Direction direction)
            => Add(new Order(new Column(schema, table, column), direction));

        /// <summary>
        /// Adds a new ordering statement using the specified column.
        /// </summary>
        /// <param name="element">The element to order by.</param>
        /// <param name="direction">The order direction.</param>
        public void Add(ISqlElement element, Direction direction)
            => Add(new Order(element, direction));

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ColumnOrderCollection"/>.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColumnOrderCollection(string column)
            => new ColumnOrderCollection { new Column(column) };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Column"/> to <see cref="ColumnOrderCollection"/>.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColumnOrderCollection(Column column)
            => new ColumnOrderCollection { column };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Order"/> to <see cref="ColumnOrderCollection"/>.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColumnOrderCollection(Order order)
            => new ColumnOrderCollection { order };
    }
}