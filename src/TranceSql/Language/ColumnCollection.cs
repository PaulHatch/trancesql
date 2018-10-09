using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class ColumnCollection : List<ISqlElement>
    {
        public void Add(string name)
        {
            Add(new Column(name));
        }

        public void Add(string table, string name)
        {
            Add(new Column(table, name));
        }

        public void Add(string schema, string table, string name)
        {
            Add(new Column(schema, table, name));
        }

        public void Add(IEnumerable<ISqlElement> elements)
        {
            AddRange(elements);
        }

        public static implicit operator ColumnCollection(string column)
            => new ColumnCollection { column };

        public static implicit operator ColumnCollection(Alias aliasedElement)
            => new ColumnCollection { aliasedElement };

        public static implicit operator ColumnCollection(ExpressionElement element)
            => new ColumnCollection { element };
    }
}
