using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a CREATE INDEX command.
    /// </summary>
    public class CreateIndex : ISqlStatement
    {
        /// <summary>
        /// Gets a value indicating whether this statement is using to create
        /// a unique constraint.
        /// </summary>
        public bool Unique { get; }

        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table this index is to be created on.
        /// </summary>
        public Table On { get; set; }

        /// <summary>
        /// Gets the columns for this index statement.
        /// </summary>
        public ColumnOrderCollection Columns { get; } = new ColumnOrderCollection();

        void ISqlElement.Render(RenderContext context)
        {
            if (Unique)
            {
                context.Write("CREATE UNIQUE INDEX ");
            }
            else
            {
                context.Write("CREATE INDEX ");
            }

            context.WriteIdentifier(Name);
            context.WriteLine();
            context.Write("ON ");
            context.Render(On);
            context.Write('(');
            context.RenderDelimited(Columns);
            context.Write(");");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Extensions.CreateDebugString(this);
    }
}
