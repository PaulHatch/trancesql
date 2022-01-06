using System.Runtime.CompilerServices;

namespace TranceSql
{
    /// <summary>
    /// This abstract class can be used to create a table schema for convenient
    /// use in query construction. This schema may contain columns or other
    /// properties of the table, and instances of the class allow implicit
    /// conversion to <see cref="Table"/> type. See online documentation for
    /// example usage.
    /// </summary>
    public abstract class TableSchema
    {
        private Table _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableSchema"/> class.
        /// </summary>
        /// <param name="schema">The table's schema.</param>
        /// <param name="name">The table name.</param>
        protected TableSchema(string schema, string name)
        {
            _table = new Table(schema, name);
        }

        /// <summary>
        /// Creates a new column for this table using the specified column name.
        /// </summary>
        /// <param name="name">The column name to use. If null or omitted the
        /// name of the property (caller) will be used instead.</param>
        /// <returns>A new column for this table.</returns>
        protected Column Column([CallerMemberName]string name = null)
            => new(_table.Schema, _table.Name, name);

        /// <summary>
        /// Creates an alias for this table.
        /// </summary>
        /// <param name="alias">The alias name.</param>
        /// <returns>A new <see cref="Alias"/> instance</returns>
        public Alias As(string alias) => _table.As(alias);

        /// <summary>
        /// Performs an implicit conversion from <see cref="TableSchema"/> to <see cref="Table"/>.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Table(TableSchema schema) => schema._table;
    }
}
