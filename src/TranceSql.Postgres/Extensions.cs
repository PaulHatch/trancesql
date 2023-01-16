using System.Collections.Generic;
using System.Linq;

namespace TranceSql.Postgres;

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
    /// Shortcut method for creating a simple 'upsert' command by adding an
    /// ON CONFLICT DO UPDATE clause to the insert statement. Any columns
    /// included in the conflict statement will be exclude from the UPDATE's
    /// SET clause. This method only supports columns as the ON CONFLICT target.
    /// For more control over how the update will be preformed, use the 
    /// <see cref="OnConflict(Insert, IEnumerable{ISqlElement}, Update)"/>
    /// method instead and supply your own update command.
    /// </summary>
    /// <param name="insert">The insert action to add to.</param>
    /// <param name="onConflict">The ON CONFLICT columns for this statement</param>
    /// <returns>A Postgres-type INSERT statement with the specified clause.</returns>
    /// <remarks>
    /// If no addition columns besides the ON CONFLICT columns are included
    /// in the source update's column list, an ON CONFLICT DO NOTHING command
    /// will be created instead.
    /// </remarks>
    public static PostgresInsert OnConflictUpdate(this Insert insert, params Column[] onConflict)
    {
        var assignments = insert.Columns
            .Zip(insert.Values, (c, v) => new Assignment(c, v))
            .Where(set =>
            {
                if (set.Target is Column column &&
                    onConflict.Any(c => c.Name == column.Name))
                {
                    return false;
                }
                return true;
            });

        var assignmentCollection = new AssignmentCollection(assignments);

        if (!assignmentCollection.Any())
        {
            return insert.OnConflictDoNothing();
        }

        return new PostgresInsert(insert)
        {
            OnConflict = new PostgresOnConflict
            {
                Target = new ColumnCollection { onConflict },
                DoUpdate = new Update
                {
                    Set = assignmentCollection
                }
            }
        };
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