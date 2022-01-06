namespace TranceSql.Postgres
{
    /// <summary>
    /// Represents a Postgres JSON path expression.
    /// </summary>
    public class JsonPath : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public JsonPath(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; }

        /// <summary>Performs an implicit conversion from <see cref="System.String"/> to <see cref="JsonPath"/>.</summary>
        /// <param name="path">The path.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator JsonPath(string path) => new(path);

        /// <summary>Performs an implicit conversion from <see cref="System.String"/> to <see cref="JsonPath"/>.</summary>
        /// <param name="path">The path.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator string(JsonPath path) => path.Path;

        void ISqlElement.Render(RenderContext context)
        {
            context.Write($"'{Path.Replace("'", "''")}'");
        }
    }
}
