using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql
{
    /// <summary>
    /// Represents a collection of values. This class supports implicit
    /// casting from <see cref="Select"/> and <see cref="Values"/> and
    /// collection initialization of values from any number of 
    /// <see cref="ISqlElement"/> to create nested <see cref="Values"/>
    /// sets. Alternatively, a <see cref="Select"/> statement can be used
    /// as the source of values for insert statements. See documentation 
    /// of the <see cref="Insert"/> command for usage examples.
    /// </summary>
    public class ValuesCollection : IEnumerable<ISqlElement>
    {
        private List<ISqlElement> _values = new List<ISqlElement>();

        /// <summary>
        /// Gets a value indicating whether this instance is select.
        /// </summary>
        public bool IsSelect => _values.Count == 1 && _values[0] is Select;

        /// <summary>
        /// Adds the specified element to this set.
        /// </summary>
        /// <param name="element">The element to add.</param>
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

        /// <summary>
        /// Adds the specified elements to this set.
        /// </summary>
        /// <param name="first">The first element to add.</param>
        /// <param name="second">The second element to add.</param>
        public void Add(ISqlElement first, ISqlElement second)
        {
            _values.Add(first);
            _values.Add(second);
        }

        /// <summary>
        /// Adds the specified elements to this set.
        /// </summary>
        /// <param name="first">The first element to add.</param>
        /// <param name="second">The second element to add.</param>
        /// <param name="third">The third element to add.</param>
        public void Add(ISqlElement first, ISqlElement second, ISqlElement third)
        {
            _values.Add(first);
            _values.Add(second);
            _values.Add(third);
        }

        /// <summary>
        /// Adds the specified object, creating a new <see cref="Value"/> if needed.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(object value) => Add(value as ISqlElement ?? new Value(value));

        /// <summary>
        /// Adds the specified objects, creating new <see cref="Value"/> instances if needed.
        /// </summary>
        /// <param name="values">The values to add.</param>
        public void Add(params object[] values)
        {
            foreach (var value in values)
            {
                Add(value as ISqlElement ?? new Value(value));
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Select"/> to <see cref="ValuesCollection"/>.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ValuesCollection(Select select) 
            => new ValuesCollection { select };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Values"/> to <see cref="ValuesCollection"/>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ValuesCollection(Values values)
            => new ValuesCollection { values as ISqlElement };

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ISqlElement> GetEnumerator() => _values.GetEnumerator();
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
    }
}