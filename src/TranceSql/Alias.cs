namespace TranceSql
{

    /// <summary>
    /// Represents an element which has an alias applied.
    /// </summary>
    public class Alias : IDataSource  // allow use in data source contexts such as FROM clause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alias"/> class.
        /// </summary>
        /// <param name="element">The element being aliased.</param>
        /// <param name="alias">The alias name.</param>
        public Alias(ISqlElement element, string alias)
        {
            Element = element;
            Name = alias;
        }

        /// <summary>
        /// Gets the element being aliased.
        /// </summary>
        public ISqlElement Element { get; }

        /// <summary>
        /// Gets the alias name.
        /// </summary>
        public string Name { get; }

        void ISqlElement.Render(RenderContext context)
        {
            if (Element is Table)
            {
                Element.Render(context);
                context.Write($" {Name}");
            }
            else
            {
                Element.Render(context);
                context.Write($" AS {Name}");
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
