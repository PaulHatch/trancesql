using System.Collections.Generic;

namespace TranceSql.Language
{
    public class AssignmentCollection : List<Assignment>
    {
        public void Add(string column, object value)
            => Add(new Assignment(new Column(column), new Value(value)));

        public void Add(string table, string column, object value)
            => Add(new Assignment(new Column(table, column), new Value(value)));

        public void Add(string column, ISqlElement value)
            => Add(new Assignment(new Column(column), value));

        public void Add(string table, string column, ISqlElement value) 
            => Add(new Assignment(new Column(table, column), value));

        public void Add(ISqlElement column, ISqlElement value)
            => Add(new Assignment(column, value));

        public static implicit operator AssignmentCollection(Assignment assignment)
            => new AssignmentCollection {  assignment };
    }
}