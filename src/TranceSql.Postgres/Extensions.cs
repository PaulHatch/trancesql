using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Postgres-specific language extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds a Postgres ON CONFLICT clause to an INSERT statement. This
        /// method will return a new <see cref="PostgresInsert"/> statement
        /// containing the specified clause using the target <see cref="Insert"/>
        /// statement.
        /// </summary>
        /// <param name="insert">The insert action to add to.</param>
        /// <param name="target">The target for the ON CONFLICT clause.</param>
        /// <param name="action">The action for the ON CONFLICT clause.</param>
        /// <returns>A Postgres-type INSERT statement with the specified clause.</returns>
        public static PostgresInsert OnConflict(
            this Insert insert,
            IEnumerable<ISqlElement> target,
            Update action)
        {
            return new PostgresInsert(insert)
            {
                OnConflict = new PostgresOnConflict
                {
                    Target = new ColumnCollection { target },
                    DoUpdate = action
                }
            };
        }

        /// <summary>
        /// Adds a Postgres ON CONFLICT DO NOTHING clause to an INSERT statement.
        /// This method will return a new <see cref="PostgresInsert"/> statement
        /// containing the specified clause using the target <see cref="Insert"/>
        /// statement.
        /// </summary>
        /// <param name="insert">The insert action to add to.</param>
        /// <returns>A Postgres-type INSERT statement with the specified clause.</returns>
        public static PostgresInsert OnConflictDoNothing(this Insert insert)
        {
            return new PostgresInsert(insert)
            {
                OnConflict = new PostgresOnConflict()
            };
        }

        // Extensions on the PostgresInsert type (i.e. not the plain Insert type)

        /// <summary>
        /// Adds a Postgres ON CONFLICT clause to an INSERT statement.
        /// statement.
        /// </summary>
        /// <param name="insert">The insert action to add to.</param>
        /// <param name="target">The target for the ON CONFLICT clause.</param>
        /// <param name="action">The action for the ON CONFLICT clause.</param>
        /// <returns>A Postgres-type INSERT statement with the specified clause.</returns>
        public static PostgresInsert OnConflict(
            this PostgresInsert insert,
            IEnumerable<ISqlElement> target,
            Update action)
        {
            insert.OnConflict = new PostgresOnConflict
            {
                Target = new ColumnCollection { target },
                DoUpdate = action
            };
            return insert;
        }

        /// <summary>
        /// Adds a Postgres ON CONFLICT DO NOTHING clause to an INSERT statement.
        /// </summary>
        /// <param name="insert">The insert action to add to.</param>
        /// <returns>A Postgres-type INSERT statement with the specified clause.</returns>
        public static PostgresInsert OnConflictDoNothing(this PostgresInsert insert)
        {
            insert.OnConflict = new PostgresOnConflict();
            return insert;
        }

        /// <summary>
        /// Renders an element to a string using default debug settings.
        /// </summary>
        /// <param name="element">The element to render.</param>
        /// <returns>String representing the specified element.</returns>
        internal static string RenderDebug(this ISqlElement element)
        {
            var debugContext = new RenderContext(new GenericDialect());
            element.Render(debugContext);
            return debugContext.CommandText;
        }
    }
}
