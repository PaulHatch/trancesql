using System.Linq;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Represents a Postgres specific insert statement which supports
    /// Postgres specific insert features.
    /// </summary>
    public class PostgresInsert : ISqlStatement
    {
        /// <summary>
        /// Gets or sets the generic insert this statement is based on.
        /// </summary>
        public Insert Insert { get; set; }

        /// <summary>
        /// Gets or sets the ON CONFLICT definition for this statement.
        /// </summary>
        public PostgresOnConflict? OnConflict { get; set; }


        internal PostgresInsert(Insert insert)
        {
            Insert = insert;
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void ISqlElement.Render(RenderContext context)
        {
            using (context.EnterChildMode(RenderMode.MultiStatment))
            {
                context.Render(Insert);
            }

            if (OnConflict != null)
            {
                context.Render(OnConflict);
            }

            var returning = Insert.Returning ?? OnConflict?.DoUpdate?.Returning;

            if (returning?.Any() == true)
            {
                context.WriteLine();
                context.Write("RETURNING ");
                context.RenderDelimited(returning);
            }

            if (context.Mode != RenderMode.MultiStatment)
            {
                context.Write(';');
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