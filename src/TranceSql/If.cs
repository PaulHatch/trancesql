namespace TranceSql
{
    /// <summary>
    /// Represents an IF statement.
    /// </summary>
    public class If : ISqlStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="If"/> class.
        /// </summary>
        public If()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="If"/> class.
        /// </summary>
        /// <param name="condition">The statement conditions.</param>
        public If(ConditionPair condition)
        {
            Condition = condition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="If"/> class.
        /// </summary>
        /// <param name="condition">The statement condition.</param>
        public If(Condition condition)
        {
            Condition = condition;
        }

        /// <summary>
        /// Gets or sets the statement conditions.
        /// </summary>
        public FilterClause Condition { get; set; }

        /// <summary>
        /// Gets or sets the statement to execute, to specify multiple statements,
        /// use a <see cref="StatementBlock"/>.
        /// </summary>
        public ISqlStatement Then { get; set; }

        /// <summary>
        /// Gets or sets the statement to execute if conditions are not met, to specify
        /// multiple statements, use a <see cref="StatementBlock"/>.
        /// </summary>
        public ISqlStatement Else { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write("IF (");
            context.Render(Condition?.Value ?? throw new InvalidCommandException("IF condition must not be null."));
            context.WriteLine(")");
            context.Render(Then ?? throw new InvalidCommandException("Then statement of IF condition must not be null."));
            if (Else != null)
            {
                context.WriteLine();
                context.WriteLine("ELSE");
                context.Render(Else);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}
