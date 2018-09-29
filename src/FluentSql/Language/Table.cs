using System;

namespace TranceSql.Language
{
    public class Table : IDataSource
    {
        public Table(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Table(string schema, string name)
            : this(name)
        {
            Schema = schema;
        }

        public string Schema { get; set; }
        public string Name { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            context.WriteIdentifierPrefix(Schema);
            context.Write(Name);
        }

        public static implicit operator Table(string table) => new Table(table);
    }
}