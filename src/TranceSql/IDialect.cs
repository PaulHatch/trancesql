using System;
using System.Collections.Generic;
using System.Data;

namespace TranceSql
{
    /// <summary>
    /// Defines a vendor specific SQL dialect which will be used to render command text.
    /// </summary>
    public interface IDialect
    {
        /// <summary>
        /// Returns a string with the provider-specific escaping applied
        /// for an identifier like a table or column name.
        /// </summary>
        /// <param name="identifier">The raw identifier name.</param>
        /// <returns>A correctly formatted string.</returns>
        string FormatIdentifier(string identifier);

        /// <summary>
        /// Gets the limit keyword behavior used by this dialect.
        /// </summary>
        LimitBehavior LimitBehavior { get; }

        /// <summary>
        /// Renders a begin transaction statement.
        /// </summary>
        /// <param name="context">The render context.</param>
        /// <param name="beginTransaction">The statement to render.</param>
        /// <remarks>
        /// The begin transaction behavior is so database-specific that we
        /// place the render implementation here rather than adding a lot of
        /// dialect specific properties.
        /// </remarks>
        void Render(RenderContext context, BeginTransaction beginTransaction);

        /// <summary>
        /// Gets the offset behavior and support used by this dialect.
        /// </summary>
        OffsetBehavior OffsetBehavior { get; }

        /// <summary>
        /// Gets the type of the output supported by this dialect.
        /// </summary>
        OutputType OutputType { get; }

        /// <summary>
        /// Formats a date constant.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>A date constant string.</returns>
        string FormatDate(DateTime date);

        /// <summary>
        /// Formats a date constant.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>A date constant string.</returns>
        string FormatDate(DateTimeOffset date);

        /// <summary>
        /// Escapes and formats a string constant.
        /// </summary>
        /// <param name="value">The string value to format.</param>
        /// <returns>A formatted string constant.</returns>
        string FormatString(string value);

        /// <summary>
        /// Creates a string representing the specified type.
        /// </summary>
        /// <param name="type">The SQL type class.</param>
        /// <param name="parameters">The type parameters, if any.</param>
        /// <returns>The name of the parameter type for this dialect.</returns>
        string FormatType(DbType type, IEnumerable<object> parameters);
    }
}