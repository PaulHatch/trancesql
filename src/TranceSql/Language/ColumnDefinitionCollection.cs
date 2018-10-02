using System.Collections;
using System.Collections.Generic;

namespace TranceSql.Language
{
    public class ColumnDefinitionCollection : List<ColumnDefinition>
    {
        public void Add(string name, SqlType sqlType)
        {
            Add(new ColumnDefinition(name, sqlType));
        }

        public void Add(IEnumerable<ColumnDefinition> columns)
        {
            AddRange(columns);
        }
    }
}