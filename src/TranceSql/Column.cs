namespace TranceSql
{
    /// <summary>
    /// Represents a table column name.
    /// </summary>
    public class Column : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Gets an "ALL" column instance, e.g. "*".
        /// </summary>
        public static Column All => new("*");

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="schema">The column's table's schema.</param>
        /// <param name="table">The column's table.</param>
        /// <param name="name">The column name. A wildcard (*) is allowed.</param>
        public Column(string? schema, string table, string name)
            : this(table, name)
        {
            Schema = schema;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column" /> class.
        /// </summary>
        /// <param name="table">The column's table.</param>
        /// <param name="name">The column name. A wildcard (*) is allowed.</param>
        public Column(string table, string name)
            :this(name)
        {
            Table = table;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column" /> class.
        /// </summary>
        /// <param name="name">The column name. A wildcard (*) is allowed.</param>
        public Column(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the column's table's schema's name. This property is optional.
        /// </summary>
        public string? Schema { get; set; }
        /// <summary>
        /// Gets or sets the column's table's name. This property is optional.
        /// </summary>
        public string? Table { get; set; }

        /// <summary>
        /// Gets or sets the column's name. This property is required. A wildcard (*) is
        /// allowed.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Produces a column using the specified table alias.
        /// </summary>
        /// <param name="tableAlias">The table alias name.</param>
        /// <returns>A column for the specified table alias.</returns>
        public Column Of(string tableAlias) => new(tableAlias, Name);

        void ISqlElement.Render(RenderContext context)
        {
            context.WriteIdentifierPrefix(Schema);
            context.WriteIdentifierPrefix(Table);
            
            if (Name == "*")
            {
                // Don't format wildcard
                context.Write('*');
            }
            else
            {
                context.WriteIdentifier(Name);
            }
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
