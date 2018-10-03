using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class Column : ExpressionElement, ISqlElement
    {
        public Column(string schema, string table, string name)
            : this(table, name)
        {
            Schema = schema;
        }

        public Column(string table, string name)
            :this(name)
        {
            Table = table;
        }

        public Column(string name)
        {
            Name = name;
        }

        public string Schema { get; set; }
        public string Table { get; set; }
        public string Name { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            context.WriteIdentifierPrefix(Schema);
            context.WriteIdentifierPrefix(Table);
            
            if (Name == "*")
            {
                // Don't format wildcard
                context.Write('*');
            }
            else
            {
                context.WriteIdentifier(Name);
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}
