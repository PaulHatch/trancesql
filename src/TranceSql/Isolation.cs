namespace TranceSql;

/// <summary>
/// Represents the isolation level of a transaction.
/// </summary>
public enum Isolation
{
    /// <summary>
    /// Indicates that statements are performed in a non-locking fashion,
    /// but a possible earlier version of a row might be used. Thus, using
    /// this isolation level, such reads are not consistent.
    /// </summary>
    ReadUncommitted,
    /// <summary>
    /// Indicates that statements can only see rows committed before it began.
    /// </summary>
    ReadCommitted,
    /// <summary>
    /// Indicates that all statements of a transaction can only see rows
    /// committed before the first query or data-modification statement
    /// was executed in this transaction.
    /// </summary>
    RepeatableRead,
    /// <summary>
    /// Indicates that data read within a transaction will never reflect
    /// changes made by other simultaneous transactions. The transaction
    /// uses the data row versions that exist when the transaction begins.
    /// No locks are placed on the data when it is read, so snapshot
    /// transactions do not block other transactions from writing data.
    /// </summary>
    Snapshot,
    /// <summary>
    /// Indicates all statements of a transaction can only see rows
    /// committed before the first query or data-modification statement
    /// was executed in this transaction.
    /// </summary>
    Serializable
}