using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TranceSql
{
    /// <summary>
    /// Represents a CREATE TABLE statement.
    /// </summary>
    public class CreateTable : ISqlStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTable"/> class.
        /// </summary>
        public CreateTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTable"/> class.
        /// </summary>
        /// <param name="name">The table name.</param>
        public CreateTable(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidCommandException("Table name must not be null");
            }

            Name = new Table(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTable"/> class.
        /// </summary>
        /// <param name="schema">The table schema.</param>
        /// <param name="name">The table name.</param>
        public CreateTable(string schema, string name)
        {
            Name = new Table(schema, name);
        }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public Table Name { get; set; }

        /// <summary>
        /// Gets the column definition collection.
        /// </summary>
        public ColumnDefinitionCollection Columns { get; } = new();

        /// <summary>
        /// Gets the table constraints collection.
        /// </summary>
        public ICollection<IConstraint> Constraints { get; } = new List<IConstraint>();

        void ISqlElement.Render(RenderContext context)
        {
            if (Name == null)
            {
                throw new InvalidCommandException("Create table statement must specify a name.");
            }

            context.Write("CREATE TABLE ");
            context.Render(Name);
            context.WriteLine();
            context.WriteLine("(");
            context.RenderDelimited(Columns, "," + context.LineDelimiter);
            foreach (var constraint in Constraints)
            {
                context.WriteLine(",");
                context.Render(constraint);
            }
            context.WriteLine();
            context.Write(");");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Extensions.CreateDebugString(this);

        /// <summary>
        /// Generates a <see cref="CreateTable" /> statement by reflecting over the specified
        /// class type using public properties which support both get and set as columns.
        /// </summary>
        /// <typeparam name="T">The class type to model</typeparam>
        /// <param name="name">The table name, if none is provided the class name will be used instead.</param>
        /// <returns></returns>
        public static CreateTable From<T>(string name)
            where T : class => From<T>(null, name);

        /// <summary>
        /// Generates a <see cref="CreateTable" /> statement by reflecting over the specified
        /// class type using public properties which support both get and set as columns.
        /// </summary>
        /// <typeparam name="T">The class type to model</typeparam>
        /// <param name="schema">The schema for the table.</param>
        /// <param name="name">The table name, if none is provided the class name will be used instead.</param>
        /// <returns></returns>
        public static CreateTable From<T>(string schema, string name)
            where T : class
        {
            var type = typeof(T);
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p => new ColumnDefinition(p.Name, SqlType.From(p.PropertyType), null));

            var table = string.IsNullOrWhiteSpace(schema) ? new Table(name ?? type.Name) : new Table(schema, name ?? type.Name);

            return new CreateTable
            {
                Name = table,
                Columns = { properties }
            };
        }
    }
}
