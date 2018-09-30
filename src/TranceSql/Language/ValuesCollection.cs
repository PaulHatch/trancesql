using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql.Language
{
    public class ValuesCollection : IEnumerable<ISqlElement>
    {
        private List<ISqlElement> _values = new List<ISqlElement>();
        
        public bool IsSelect => _values.Count == 1 && _values[0] is Select;

        public void Add(ISqlElement element)
        {
            if (IsSelect)
            {
                throw new InvalidCommandException("Only one select can be specified for an insert statement.");
            }

            if (element is Select)
            {
                if (_values.Any())
                {
                    throw new InvalidCommandException("A select statement must be the only values source in an insert statement.");
                }

                _values.Add(element);
            }
            else
            {
                _values.Add(element);
            }
        }

        public void Add(ISqlElement first, ISqlElement second)
        {
            _values.Add(first);
            _values.Add(second);
        }

        public void Add(ISqlElement first, ISqlElement second, ISqlElement third)
        {
            _values.Add(first);
            _values.Add(second);
            _values.Add(third);
        }

        public void Add(object value) => Add(value as ISqlElement ?? new Value(value));

        public void Add(params object[] values)
        {
            foreach (var value in values)
            {
                Add(value as ISqlElement ?? new Value(value));
            }
        }

        public static implicit operator ValuesCollection(Select select) 
            => new ValuesCollection { select };

        public static implicit operator ValuesCollection(Values values)
            => new ValuesCollection { values as ISqlElement };

        public IEnumerator<ISqlElement> GetEnumerator() => _values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
    }
}