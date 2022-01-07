using System;
using System.Collections.Generic;
using System.Data;

namespace TranceSql
{
    /// <summary>
    /// Represents a CAST expression.
    /// </summary>
    public class Cast : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Gets or sets the expression being cast.
        /// </summary>
        public ISqlElement Expression { get; set; }

        /// <summary>
        /// Represents a CAST expression using a raw string type value as the
        /// target type.
        /// </summary>
        /// <param name="expression">The expression to be cast.</param>
        /// <param name="rawType">Type to be converted. Note that this value
        /// is vulnerable to injection attacks if untrusted input is used. If
        /// this value contains an apostrophe, an exception will be thrown as a
        /// is a precaution, but is insufficient to prevent all possible
        /// injection attacks.</param>
        public Cast(ISqlElement expression, string rawType)
        {
            if (rawType == null)
            {
                throw new ArgumentNullException("Type value must not be null.", nameof(rawType));
            }
            if (String.IsNullOrWhiteSpace(rawType) || rawType.Contains("'"))
            {
                throw new ArgumentException("Invalid type value string.", nameof(rawType));
            }

            Expression = expression ?? throw new ArgumentNullException("Expression must not be null.", nameof(expression));
            AsRaw = rawType;
        }

        /// <summary>
        /// Represents a CAST expression using a DB type and optional type
        /// parameters. The specific value used will be 
        /// </summary>
        /// <param name="expression">The expression to be cast.</param>
        /// <param name="type">The DB type to cast to.</param>
        /// <param name="typeParameters">Any parameters for the cast type, such as length of a char type.</param>
        public Cast(ISqlElement expression, DbType type, params object[] typeParameters)
        {
            Expression = expression ?? throw new ArgumentNullException("Expression must not be null.", nameof(expression));
            As = type;
            AsParams = typeParameters;
        }

        /// <summary>
        /// Gets as raw type to be cast to.
        /// </summary>
        public string? AsRaw { get; }

        /// <summary>
        /// Gets the DB type to be cast to.
        /// </summary>
        public DbType? As { get; }

        /// <summary>
        /// Gets any parameters for the cast type, such as length of a char type.
        /// </summary>
        public IEnumerable<object>? AsParams { get; }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write("CAST(");
            context.Render(Expression);
            context.Write(" AS ");
            if (As.HasValue)
            {
                context.Write(context.Dialect.FormatType(As.Value, AsParams));
            }
            else if(AsRaw != null)
            {
                context.Write(AsRaw);
            } else {
                throw new InvalidCommandException("Either 'As' or 'AsRaw' type name must be specified in cast expression.");
            }

            context.Write(')');
        }
    }
}
