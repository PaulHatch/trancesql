using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a parameter in a SQL statement.
    /// </summary>
    public class Parameter : ExpressionElement, ISqlElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name. The name will automatically be prefixed with '@' if none is present.
        /// </param>
        public Parameter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter() { }

        private string _name;

        /// <summary>
        /// Gets or sets the name. The name will automatically be prefixed with 
        /// '@' if none is present.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value.StartsWith("@") ? value : $"@{value}";
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write(Name ?? throw new InvalidOperationException("Parameter name cannot be null"));
        }

        public override string ToString() => this.RenderDebug();
    }
}
