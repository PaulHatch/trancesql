using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a CREATE DATABASE statement.
    /// </summary>
    public class CreateDatabase : ISqlStatement
    {
        /// <summary>
        /// Gets the name of the database to create.
        /// </summary>
        public string DatabaseName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabase"/> class.
        /// </summary>
        /// <param name="name">The name of the database to create.</param>
        public CreateDatabase(string name)
        {
            DatabaseName = name;
        }

        void ISqlElement.Render(RenderContext context)
        {
            context.Write("CREATE DATABASE ");
            context.WriteIdentifier(DatabaseName);
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
