using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents an ordering of either ASC or DESC applied to an element.
    /// </summary>
    public class Order : ISqlElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="element">The element to wrap.</param>
        /// <param name="direction">The direction for this order fragment.</param>
        public Order(ISqlElement element, Direction direction)
        {
            Element = element;
            Direction = direction;
        }

        /// <summary>
        /// Gets the element this order wraps.
        /// </summary>
        public ISqlElement Element { get; }

        /// <summary>
        /// Gets the direction of this order element.
        /// </summary>
        public Direction Direction { get; }

        void ISqlElement.Render(RenderContext context)
        {
            Element.Render(context);
            switch (Direction)
            {
                case Direction.Ascending:
                    context.Write(" ASC");
                    break;
                case Direction.Descending:
                    context.Write(" DESC");
                    break;
                default:
                    break;
            }
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
