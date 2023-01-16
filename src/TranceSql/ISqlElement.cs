namespace TranceSql;

/// <summary>
/// Defines a fragment of SQL which can be rendered.
/// </summary>
public interface ISqlElement
{
    /// <summary>
    /// Called to render this element to the specified render context for a SQL command. This
    /// method will be called only once for each time they appear in a command definition, but 
    /// implementations should still be  idempotent if they might be reused during command 
    /// generation, for example the same command or column could be reused by callers. SQL
    /// elements *must not* store state related to the render process, any such data must be
    /// stored only in the render context itself.
    /// </summary>
    /// <param name="context">The render context.</param>
    void Render(RenderContext context);
}