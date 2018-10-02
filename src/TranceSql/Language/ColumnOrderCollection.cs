namespace TranceSql.Language
{
    public class ColumnOrderCollection : ColumnCollection
    {
        public void Add(string name, Direction direction)
            => Add(new Order(new Column(name), direction));

        public void Add(string table, string name, Direction direction)
            => Add(new Order(new Column(table, name), direction));

        public void Add(string schema, string table, string name, Direction direction)
            => Add(new Order(new Column(schema, table, name), direction));

        public void Add(ISqlElement element, Direction direction)
            => Add(new Order(element, direction));
    }
}