using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a value which will be passed to the final command as a dynamic parameter.
    /// A value may be reused, only the initial value will be
    /// </summary>
    public class Value : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Gets the argument for this value.
        /// </summary>
        public object Argument { get; }

        /// <summary>
        /// Invalid operation, SQL elements are not valid as values for dynamic
        /// parameters. This constructor will thrown an exception if called.
        /// </summary>
        /// <param name="value">The value.</param>
        [Obsolete("SQL elements are not valid as values for dynamic parameters.", true)]
        public Value(ISqlElement value)
        {
            throw new InvalidCommandException("Attempted to pass an instance of an ISqlElement as a value in query, SQL elements should not be passed as values.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Value(object value)
        {
            if (value is ISqlElement)
            {
                throw new InvalidCommandException("Attempted to pass an instance of an ISqlElement as a value in query, SQL elements should not be passed as values.");
            }

            Argument = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write(context.GetParameter(this));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}
