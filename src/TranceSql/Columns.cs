using System.Collections.Generic;

namespace TranceSql
{
    /// <summary>
    /// Represents several columns that are part of a clause.
    /// </summary>
    public class Columns : ISqlElement
    {
        /// <summary>
        /// Gets columns in this list.
        /// </summary>
        public IList<Column> Items { get; } = new List<Column>();

        /// <summary>
        /// Creates a new columns list.
        /// </summary>
        /// <param name="columns">Columns to include.</param>
        public Columns(IEnumerable<Column> columns) => (Items as List<Column>)!.AddRange(columns);

        /// <summary>
        /// Creates a new columns list.
        /// </summary>
        /// <param name="columns">Columns to include.</param>
        public Columns(params Column[] columns) => (Items as List<Column>)!.AddRange(columns);

        /// <summary>
        /// Creates a new columns list.
        /// </summary>
        public Columns() { }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write('(');
            context.RenderDelimited(Items);
            context.Write(')');
        }

        /// <summary>Converts list of to a columns type.</summary>
        /// <param name="columns">List of columns to convert.</param>
        public static implicit operator Columns(List<Column> columns) => new(columns);

        /// <summary>Converts list of to a columns type.</summary>
        /// <param name="columns">List of columns to convert.</param>
        public static implicit operator Columns(Column[] columns) => new(columns);
    }
}
