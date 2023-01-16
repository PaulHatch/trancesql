namespace TranceSql;

/// <summary>
/// Represents a ROLLBACK TRANSACTION statement.
/// </summary>
public class RollbackTransaction : ISqlStatement
{
    void ISqlElement.Render(RenderContext context)
    {
        context.Write("ROLLBACK TRANSACTION;");
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}