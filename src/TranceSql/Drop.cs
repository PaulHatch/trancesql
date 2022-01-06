using System;

namespace TranceSql
{
    /// <summary>
    /// Represents a DROP statement.
    /// </summary>
    public class Drop : ISqlStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Drop"/> class.
        /// </summary>
        /// <param name="table">The table to drop.</param>
        public Drop(Table table)
        {
            Type = DropType.Table;
            Target = table;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Drop"/> class.
        /// </summary>
        /// <param name="type">The type of drop statement.</param>
        /// <param name="target">The element to drop.</param>
        public Drop(DropType type, ISqlElement target)
        {
            Type = type;
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Drop"/> class.
        /// </summary>
        /// <param name="type">The type of drop statement.</param>
        /// <param name="target">The element to drop.</param>
        public Drop(DropType type, string target)
        {
            Type = type;
            switch (Type)
            {
                case DropType.Table:
                    Target = new Table(target);
                    break;
                default:
                    throw new InvalidCommandException($"Cannot convert string to drop type '{Type}'");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Drop" /> class.
        /// </summary>
        /// <param name="type">The drop type.</param>
        /// <param name="schema">The table schema.</param>
        /// <param name="target">The target name.</param>
        public Drop(DropType type, string schema, string target)
        {
            Type = type;
            switch (Type)
            {
                case DropType.Table:
                    Target = new Table(schema, target);
                    break;
                default:
                    throw new InvalidCommandException($"Cannot convert schema+name strings to drop type '{Type}'");
            }
        }

        /// <summary>
        /// Gets the type of element to drop.
        /// </summary>
        public DropType Type { get; }

        /// <summary>
        /// Gets the target element to drop.
        /// </summary>
        public ISqlElement Target { get; }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write("DROP ");
            switch (Type)
            {
                case DropType.Table:
                    context.Write("TABLE ");
                    break;
                case DropType.Constraint:
                    throw new NotImplementedException();
                default:
                    throw new InvalidCommandException($"Unrecognized drop type '{Type}'");
            }
            context.Render(Target);
            context.Write(';');
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
