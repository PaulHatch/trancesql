using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    public class Values : IEnumerable<ISqlElement>, ISqlElement
    {
        List<ISqlElement> _data = new List<ISqlElement>();
        public ICollection<ISqlElement> Data => _data;

        public Values()
        {
        }

        public Values(IEnumerable<object> values)
        {
            var data = values?.Select(v => v is ISqlElement element ? element : new Value(v));
            if (data != null)
            {
                _data.AddRange(data);
            }
        }

        public Values(IEnumerable<ISqlElement> values)
        {
            _data.AddRange(values);
        }

        public void Add(ISqlElement element) => _data.Add(element);
        public void Add(object value) => _data.Add(value is ISqlElement el ? el : new Value(value));


        void ISqlElement.Render(RenderContext context)
        {
            context.Write('(');

            var first = true;
            foreach (var value in Data)
            {
                if (!first)
                {
                    context.Write(", ");
                }
                else
                {
                    first = false;
                }

                context.Render(value);

            }

            context.Write(')');
        }

        public IEnumerator<ISqlElement> GetEnumerator() => _data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        public override string ToString() => this.RenderDebug();
    }
}
