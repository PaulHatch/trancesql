using System.Collections.Generic;

namespace TranceSql
{
    /// <summary>
    /// Represents a block of statements wrapped by BEGIN and END.
    /// </summary>
    public class StatementBlock : List<ISqlStatement>, ISqlStatement
    {
        void ISqlElement.Render(RenderContext context)
        {
            context.WriteLine("BEGIN");
            context.RenderDelimited(this, context.LineDelimiter);
            context.WriteLine();
            context.Write("END");
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
