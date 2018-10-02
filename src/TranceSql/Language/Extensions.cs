using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public static class Extensions
    {
        public static Alias As(this ISqlElement element, string alias)
            => new Alias(element, alias);

        public static Order Asc(this ISqlElement element)
            => new Order(element, Direction.Ascending);

        public static Order Desc(this ISqlElement element)
            => new Order(element, Direction.Descending);

        internal static string RenderDebug(this ISqlElement element)
        {
            var debugContext = new RenderContext(new GenericDialect());
            element.Render(debugContext);
            return debugContext.CommandText;
        }
    }
}
