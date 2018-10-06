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

        public static Function Count() => new Function("COUNT", new Column("*"));
        public static Function Max(ISqlElement element) => new Function("MAX", element);
        public static Function Min(ISqlElement element) => new Function("MIN", element);
        public static Function Sum(ISqlElement element) => new Function("SUM", element);
        public static Function Coalesce(params ISqlElement[] elements) => new Function("COALESCE", elements);

        public static Function Avg(ISqlElement element) => new Function("AVG", element);
        public static Function Abs(ISqlElement element) => new Function("ABG", element);
        public static Function Ceiling(ISqlElement element) => new Function("CEILING", element);
        public static Function Floor(ISqlElement element) => new Function("Floor", element);

    }
}
