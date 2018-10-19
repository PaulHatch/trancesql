using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a collection of data sources such as <see cref="Table"/> elements. This
    /// class supports implicit casting from <see cref="String"/>, <see cref="Alias"/>,
    /// <see cref="Table"/>, or <see cref="Select"/> class add well as collection initialization
    /// of tables from string values. See documentation of the <see cref="Select"/> command for
    /// usage examples.
    /// </summary>
    public class DataSourceCollection : List<IDataSource>
    {
        /// <summary>
        /// Adds a table using the specified name.
        /// </summary>
        /// <param name="name">The table name.</param>
        public void Add(string name)
        {
            Add(new Table(name));
        }

        /// <summary>
        /// Adds a table using the specified name.
        /// </summary>
        /// <param name="schema">The table schema.</param>
        /// <param name="table">The table.</param>
        public void Add(string schema, string table)
        {
            Add(new Table(schema, table));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="DataSourceCollection"/>.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator DataSourceCollection(string table)
            => new DataSourceCollection { table };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Table"/> to <see cref="DataSourceCollection"/>.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator DataSourceCollection(Table table)
            => new DataSourceCollection { table };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Select"/> to <see cref="DataSourceCollection"/>.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator DataSourceCollection(Select select)
            => new DataSourceCollection { select };

        /// <summary>
        /// Performs an implicit conversion from <see cref="Alias"/> to <see cref="DataSourceCollection"/>.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator DataSourceCollection(Alias table)
            => new DataSourceCollection { table };
    }
}
