using System;
using System.Collections.Generic;

namespace TranceSql.Language
{
    /// <summary>
    /// Column definition fragment from a create or alter table statement.
    /// </summary>
    public class ColumnDefinition : ISqlElement
    {
        private string Name { get; }
        private SqlType Type { get; }
        public IEnumerable<IConstraint> Constraints { get; }

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

        public override string ToString() => this.RenderDebug();
    }
}