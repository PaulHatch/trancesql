using System;
using System.Collections.Generic;

namespace TranceSql
{
    /// <summary>
    /// Column definition fragment from a create or alter table statement.
    /// </summary>
    public class ColumnDefinition : ISqlElement
    {
        private string Name { get; }
        private SqlType Type { get; }
        
        /// <summary>
        /// Gets the constraints defined for this column.
        /// </summary>
        public IEnumerable<IConstraint> Constraints { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="name">The column name. Must not be null or whitespace.</param>
        /// <param name="sqlType">The SQL type of the column. Must not be null.</param>
        /// <param name="constraints">The column constraints.</param>
        public ColumnDefinition(string name, SqlType sqlType, IEnumerable<IConstraint> constraints)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Column name cannot be null or whitespace", nameof(name));
            }

            Name = name;
            Type = sqlType ?? throw new ArgumentNullException(nameof(sqlType));
            Constraints = constraints ?? Array.Empty<IConstraint>();
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.WriteIdentifier(Name);
            context.Write(' ');
            context.Render(Type);
            foreach (var constraint in Constraints)
            {
                context.Write(' ');
                context.Render(constraint);
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