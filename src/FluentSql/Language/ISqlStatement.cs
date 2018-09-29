namespace TranceSql.Language
{
    /// <summary>
    /// Defines a SQL element which can function as a top-level object and be added to command. All
    /// statements are also SQL elements.
    /// </summary>
    /// <seealso cref="TranceSql.Language.ISqlElement" />
    public interface ISqlStatement : ISqlElement
    {
    }
}
