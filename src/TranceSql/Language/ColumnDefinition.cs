using System;

namespace TranceSql.Language
{
    /// <summary>
    /// Column definition fragment from a create or alter table statement.
    /// </summary>
    public class ColumnDefinition : ISqlElement
    {
        private string Name { get; }
        private SqlType Type { get; }

        public ColumnDefinition(string name, SqlType sqlType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Column name cannot be null or whitespace", nameof(name));
            }

            Name = name;
            Type = sqlType ?? throw new ArgumentNullException(nameof(sqlType));
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write(context.Dialect.FormatIdentifier(Name));
            context.Write(' ');
            context.Render(Type);
        }

        public override string ToString() => this.RenderDebug();
    }
}