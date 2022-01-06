namespace TranceSql
{
    /// <summary>
    /// Represents a COMMIT TRANSACTION statement.
    /// </summary>
    public class CommitTransaction : ISqlStatement
    {
        void ISqlElement.Render(RenderContext context)
        {
            context.Write("COMMIT TRANSACTION;");
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
