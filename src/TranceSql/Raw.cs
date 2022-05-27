namespace TranceSql
{
    /// <summary>
    /// Represents a raw snippet of SQL code.
    /// </summary>
    public class Raw : ExpressionElement, ISqlStatement
    {
        /// <summary>
        /// Gets or sets the string value for this element.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Raw"/> class.
        /// </summary>
        /// <param name="value">The raw string value.</param>
        public Raw(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Raw"/> class.
        /// </summary>
        public Raw()
        {
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (Value is not null)
            {
                context.Write(Value);
            }
        }
    }
}