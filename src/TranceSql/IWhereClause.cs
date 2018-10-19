namespace TranceSql
{
    /// <summary>
    /// Defines a <see cref="ISqlElement"/> which can be used as a condition.
    /// </summary>
    public interface ICondition : ISqlElement
    {
        /// <summary>
        /// Gets or sets the operator for this condition.
        /// </summary>
        BooleanOperator BooleanOperator { get; set;  }
    }
}
