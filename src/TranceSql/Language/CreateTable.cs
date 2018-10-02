using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class CreateTable : ISqlStatement
    {
        public CreateTable(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidCommandException("Table name must not be null");
            }

            Name = new Table(name);
        }

        public CreateTable(string schema, string name)
        {
            Name = new Table(schema, name);
        }

        public Table Name { get; set; }

        public ColumnDefinitionCollection Columns { get; } = new ColumnDefinitionCollection();

        void ISqlElement.Render(RenderContext context)
        {
            context.Write("CREATE TABLE ");
            context.Render(Name);
            context.WriteLine();
            context.WriteLine("(");
            context.RenderDelimited(Columns, "," + context.LineDelimiter);
            context.WriteLine();
            context.Write(");");
        }

        public override string ToString() => this.RenderDebug();
    }
}
