using System;
using System.Collections.Generic;
using System.Text;

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
            => new JsonExpression(element, JsonOperator.Get, value);

        /// <summary>Creates '-&gt;&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGetAsText(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.GetAsText, value);

        /// <summary>Creates '#&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGetByPath(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.GetByPath, value);

        /// <summary>Creates '#&gt;&gt;' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonGetByPathAsText(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.GetByPathAsText, value);

        /// <summary>Creates '@>' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonLeftContainsRight(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.LeftContainsRight, value);

        /// <summary>Creates '&lt;@' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonRightContainsLeft(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.RightContainsLeft, value);

        /// <summary>Creates '?' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonContains(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.Contains, value);

        /// <summary>Creates '?|' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonContainsAny(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.ContainsAny, value);

        /// <summary>Creates '||' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonConcat(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.Concat, value);

        /// <summary>Creates '-' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonDelete(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.Delete, value);

        /// <summary>Creates '#-' operation on this value.</summary>
        /// <param name="element">The left side value of the expression.</param>
        /// <param name="value">The right side value of the expression.</param>
        /// <returns>JSON expression instance for the specified operation.</returns>
        public static JsonExpression JsonDeletePath(this ExpressionElement element, ExpressionElement value)
            => new JsonExpression(element, JsonOperator.DeletePath, value);
    }
}
