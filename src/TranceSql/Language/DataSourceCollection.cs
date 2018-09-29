using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class DataSourceCollection : List<IDataSource>
    {
        public void Add(string name)
        {
            Add(new Table(name));
        }

        public void Add(string schema, string table)
        {
            Add(new Table(schema, table));
        }

        public static implicit operator DataSourceCollection(string table)
            => new DataSourceCollection { table };

        public static implicit operator DataSourceCollection(Table table)
            => new DataSourceCollection { table };

        public static implicit operator DataSourceCollection(Select select)
            => new DataSourceCollection { select };

        public static implicit operator DataSourceCollection(Alias table)
            => new DataSourceCollection { table };
    }
}
