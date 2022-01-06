namespace TranceSql.Postgres
{
    /// <summary>
    /// Provides extension helper methods for expression elements to create 
    /// Postgres JSON expressions for json or jsonb data types.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>Creates '-&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGet(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.Get, value);

        /// <summary>Creates '-&gt;&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGetAsText(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.GetAsText, value);

        /// <summary>Creates '#&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGetByPath(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.GetByPath, value);

        /// <summary>Creates '#&gt;&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGetByPathAsText(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.GetByPathAsText, value);

        /// <summary>Creates '@>' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonCondition JsonLeftContainsRight(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonConditionOperator.LeftContainsRight, value);

        /// <summary>Creates '&lt;@' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonCondition JsonRightContainsLeft(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonConditionOperator.RightContainsLeft, value);

        /// <summary>Creates '?' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonCondition JsonContains(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonConditionOperator.Contains, value);

        /// <summary>Creates '?|' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonCondition JsonContainsAny(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonConditionOperator.ContainsAny, value);

        /// <summary>Creates '||' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonConcat(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.Concat, value);

        /// <summary>Creates '-' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonDelete(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.Delete, value);

        /// <summary>Creates '#-' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonDeletePath(this ExpressionElement element, ExpressionElement value)
            => new(element, JsonExpressionOperator.DeletePath, value);
    }
}
