using System;

namespace TranceSql
{
    /// <summary>
    /// Represents a table name.
    /// </summary>
    public class Table : IDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="name">The table name, must not be null.</param>
        public Table(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        /// <param name="schema">The table's schema.</param>
        /// <param name="name">The table name, must not be null.</param>
        /// <exception cref="ArgumentNullException">schema</exception>
        public Table(string schema, string name)
            : this(name)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        /// <summary>
        /// Gets or sets the table's schema.
        /// </summary>
        public string? Schema { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public string Name { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            context.WriteIdentifierPrefix(Schema);
            context.Write(Name);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Table"/>.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Table(string table) => new(table);
    }
}