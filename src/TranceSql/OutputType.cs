namespace TranceSql
{
    /// <summary>
    /// Represents the type of output supported by a SQL dialect.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// Indicates that output is not supported.
        /// </summary>
        None,
        /// <summary>
        /// Indicates the RETURNING keyword is used for output.
        /// </summary>
        Returning,
        /// <summary>
        /// Indicates the OUTPUT keyword is used for output.
        /// </summary>
        Output
    }
}