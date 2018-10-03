using System.Collections;
using System.Collections.Generic;

namespace TranceSql.Language
{
    public class ColumnDefinitionCollection : List<ColumnDefinition>
    {
        public void Add(string name, SqlType sqlType)
        {
            Add(new ColumnDefinition(name, sqlType, null));
        }

        public void Add(string name, SqlType sqlType, IConstraint constraint)
        {
            Add(new ColumnDefinition(name, sqlType, new[] { constraint }));
        }

        public void Add(string name, SqlType sqlType, params IConstraint[] constraints)
        {
            Add(new ColumnDefinition(name, sqlType, constraints));
        }

        public void Add(IEnumerable<ColumnDefinition> columns)
        {
            AddRange(columns);
        }
    }
}