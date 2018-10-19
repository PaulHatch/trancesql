namespace TranceSql
{
    /// <summary>
    /// Represents the operator for a <see cref="BinaryExpression" />
    /// </summary>
    public enum ArithmeticOperator
    {
        /// <summary>
        /// Indicates an add (+) operator.
        /// </summary>
        Add,
        /// <summary>
        /// Indicates a subtract (-) operator.
        /// </summary>
        Subtract,
        /// <summary>
        /// Indicates a multiple (*) operator.
        /// </summary>
        Multiply,
        /// <summary>
        /// Indicates a divide (/) operator.
        /// </summary>
        Divide,
        /// <summary>
        /// Indicates a modulo (%) operator.
        /// </summary>
        Modulo
    }
}