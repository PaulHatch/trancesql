using System;
using System.Collections.Generic;
using System.Text;

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
        public PostgresOnConflict OnConflict { get; set; }

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

            context.Render(OnConflict);
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
