namespace TranceSql.Language
{
    /// <summary>
    /// Represents the mode to render SQL elements in.
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// Indicates that a full statement is being rendered.
        /// </summary>
        Statment,
        /// <summary>
        /// Indicates that a nested statement is being rendered.
        /// </summary>
        Nested,
        /// <summary>
        /// Indicates that multiple statements are being rendered together
        /// such as in a union statement.
        /// </summary>
        MultiStatment
    }
}