namespace TranceSql;

/// <summary>
/// Represents a BEGIN TRANSACTION statement.
/// </summary>
public class BeginTransaction : ISqlStatement
{
    /// <summary>
    /// Represents a BEGIN TRANSACTION statement.
    /// </summary>
    public BeginTransaction() {}

    /// <summary>
    /// Represents a BEGIN TRANSACTION statement with an explicit
    /// isolation level. For certain SQL dialects specifying an isolation
    /// level may result in a SET TRANSACTION ISOLATION LEVEL being added
    /// immediately before this statement.
    /// </summary>
    /// <param name="isolation">The isolation level for the command,
    /// some levels are only supported by certain SQL dialects.</param>
    public BeginTransaction(Isolation isolation)
    {
        Isolation = isolation;
    }

    /// <summary>
    /// Gets or sets the isolation level of this transaction.
    /// </summary>
    public Isolation? Isolation { get; set; }

    /// <summary>
    /// Gets or sets whether this transaction should be created as read
    /// only.
    /// </summary>
    public bool? ReadOnly { get; set; }

    /// <summary>
    /// Renders the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    void ISqlElement.Render(RenderContext context)
    {
        context.Dialect.Render(context, this);
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}