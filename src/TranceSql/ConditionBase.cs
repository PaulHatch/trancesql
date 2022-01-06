namespace TranceSql
{
    /// <summary>
    /// Defines a <see cref="ISqlElement"/> which can be used as a condition.
    /// </summary>
    public abstract class ConditionBase : ExpressionElement
    {
        /// <summary>
        /// Combines two conditions using AND.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// A new condition collection.
        /// </returns>
        public static ConditionPair operator &(ConditionBase left, ConditionBase right) => ConditionPair.And(left, right);

        // Some of the bool operators are here, the reset are in ConditionCollection class

        /// <summary>
        /// Combines two conditions using OR.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>
        /// A new condition collection.
        /// </returns>
        public static ConditionPair operator |(ConditionBase left, ConditionBase right) => ConditionPair.Or(left, right);
    }
}
