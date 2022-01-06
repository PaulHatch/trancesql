using System;
using System.Collections;
using System.Collections.Generic;

namespace TranceSql
{
    /// <summary>
    /// Represents a set of values, used in an <see cref="Insert"/> statement. In 
    /// a <see cref="ValuesCollection"/> this can be used to indicate nesting for
    /// multiple rows.
    /// </summary>
    public class Values : IEnumerable<ISqlElement>, ISqlElement
    {
        List<ISqlElement> _data = new();
        
        /// <summary>
        /// Gets the data. (This property would be named "Values" but C# does not
        /// allow properties to have the same name as the type that defines them.)
        /// </summary>
        public ICollection<ISqlElement> Data => _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Values"/> class.
        /// </summary>
        public Values()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values"/> class.
        /// </summary>
        /// <param name="values">The values for this set.</param>
        public Values(IEnumerable values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            foreach (var value in values)
            {
                _data.Add(value as ISqlElement ?? new Value(value));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values"/> class.
        /// </summary>
        /// <param name="values">The values for this set.</param>
        public Values(IEnumerable<ISqlElement> values)
        {
            _data.AddRange(values);
        }

        /// <summary>
        /// Adds the specified element to the set.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void Add(ISqlElement element) => _data.Add(element);

        /// <summary>
        /// Adds the specified element to the set.
        /// </summary>
        /// <param name="value">The value to add.</param>
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ISqlElement> GetEnumerator() => _data.GetEnumerator();
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}
