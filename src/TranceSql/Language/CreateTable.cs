using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TranceSql.Language
{
    public class CreateTable : ISqlStatement
    {
        public CreateTable()
        {
        }

        public CreateTable(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidCommandException("Table name must not be null");
            }

            Name = new Table(name);
        }

        public CreateTable(string schema, string name)
        {
            Name = new Table(schema, name);
        }

        public Table Name { get; set; }

        public ColumnDefinitionCollection Columns { get; } = new ColumnDefinitionCollection();

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
            context.WriteLine();
            context.Write(");");
        }

        public override string ToString() => this.RenderDebug();

        /// <summary>
        /// Generates a <see cref="CreateTable"/> statement by reflecting over the specified
        /// class type using public properties which support both get and set as columns.
        /// </summary>
        /// <typeparam name="T">The class type to model</typeparam>
        /// <param name="name">The table name, if none is provided the class name will be used instead.</param>
        /// <returns></returns>
        public static CreateTable From<T>(string schema = null, string name = null)
            where T : class
        {
            var type = typeof(T);
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p => new ColumnDefinition(p.Name, SqlType.From(p.PropertyType)));

            var table = String.IsNullOrWhiteSpace(schema) ? new Table(name ?? type.Name) : new Table(schema, name ?? type.Name);

            return new CreateTable
            {
                Name = table,
                Columns = { properties }
            };
        }
    }
}
