namespace TranceSql
{
    /// <summary>
    /// Represents limit behavior used by a SQL dialect.
    /// </summary>
    public enum LimitBehavior
    {
        /// <summary>
        /// FETCH FIRST behavior.
        /// </summary>
        FetchFirst,
        /// <summary>
        /// TOP behavior.
        /// </summary>
        Top,
        /// <summary>
        /// LIMIT behavior.
        /// </summary>
        Limit,
        /// <summary>
        /// ROWNUMBER behavior.
        /// </summary>
        RowNum,
        /// <summary>
        /// ROWNUMBER with automatic behavior.
        /// </summary>
        RowNumAutomatic
    }
}