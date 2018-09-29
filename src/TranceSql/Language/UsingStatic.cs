using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public static class UsingStatic
    {
        public static Column Column(string name) => new Column(name);
        public static Column Column(string table, string name) => new Column(table, name);
        public static Column Column(string schema, string table, string name) => new Column(schema, table, name);

        public static Table Table(string table) => new Table(table);
        public static Table Table(string schema, string table) => new Table(schema, table);

        public static Parameter Parameter(string name) => new Parameter(name);

        public static Value Value(object value) => new Value(value);

        public static Values Values(object value) => new Values { value };
        public static Values Values(object first, object second) => new Values { first, second };
        public static Values Values(object first, object second, object third) => new Values { first, second, third };
        public static Values Values(params object[] values) => new Values(values);

    }
}
