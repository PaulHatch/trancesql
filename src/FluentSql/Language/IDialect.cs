using System;

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
    }
}