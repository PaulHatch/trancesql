using System;
using System.Data;

namespace TranceSql.Language
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
        /// Gets the offset behavior and support used by this dialect.
        /// </summary>
        OffsetBehavior OffsetBehavior { get; }

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
        /// <param name="parameter">The type parameter, if any.</param>
        /// <returns></returns>
        string FormatType(DbType type, int? parameter);
    }
}