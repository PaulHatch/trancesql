namespace TranceSql;

/// <summary>
/// Represents the type of element in a <see cref="Drop"/> statement.
/// </summary>
public enum DropType
{
    /// <summary>
    /// A DROP TABLE statement.
    /// </summary>
    Table,
    /// <summary>
    /// A DROP CONSTRAINT statement.
    /// </summary>
    Constraint,
    /// <summary>
    /// A DROP INDEX statement.
    /// </summary>
    Index
}