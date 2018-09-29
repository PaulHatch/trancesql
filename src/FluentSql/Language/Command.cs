using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql.Language
{
    public class Command : IEnumerable<ISqlStatement>
    {
        private List<ISqlStatement> _statements = new List<ISqlStatement>();
        private SqlCommandManager _transaction;
        private DeferContext _context;
        internal IDialect Dialect { get; }

        /// <summary>
        /// Adds the specified statement to this command.
        /// </summary>
        /// <param name="statement">The statement to add.</param>
        public void Add(ISqlStatement statement) => _statements.Add(statement);

        #region Execution

        /// <summary>
        /// Executes the current command and returns the result as 
        /// an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <returns>Result of command as a list.</returns>
        public Task<IEnumerable<TResult>> FetchListAsync<TResult>()
        {
            return _transaction.ExecuteListResultAsync<TResult>(Render());
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="defaultValue">Value to return if result is null or command returns no values.</param>
        /// <returns>Result of command.</returns>
        public Task<TResult> FetchAsync<TResult>(TResult defaultValue = default(TResult))
        {
            return _transaction.ExecuteResultAsync<TResult>(Render(), defaultValue, null);
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult> FetchAsync<TResult>(params Expression<Func<TResult, IEnumerable>>[] collections)
        {
            return _transaction.ExecuteResultAsync<TResult>(Render(), default(TResult), collections.Select(c => c.GetPropertyInfo()));
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult> FetchAsync<TResult>(IEnumerable<PropertyInfo> collections)
        {
            if (!collections.All(p => p.PropertyType.ImplementsInterface<IEnumerable>()))
                throw new ArgumentException("All properties must be collections", "collections");

            return _transaction.ExecuteResultAsync<TResult>(Render(), default(TResult), collections);
        }

        /// <summary>
        /// Executes the current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult> FetchMappedResultAsync<TResult>(params Expression<Func<TResult, object>>[] map)
            where TResult : new()
        {
            var mappedProperties = MapProperties<TResult>(map);
            return _transaction.ExecuteMapResultAsync<TResult>(Render(), mappedProperties);
        }

        /// <summary>
        /// Maps a list of properties expression to properties and types.
        /// </summary>
        /// <typeparam name="TResult">The entity type to select.</typeparam>
        /// <param name="map">A list of property select expressions.</param>
        /// <returns>A list of properties and their types.</returns>
        private static IEnumerable<Tuple<PropertyInfo, Type>> MapProperties<TResult>(IEnumerable<Expression<Func<TResult, object>>> map)
        {
            return MapProperties(map.Select(c => c.GetPropertyInfo()));
        }

        /// <summary>
        /// Maps a list of properties to their property types.
        /// </summary>
        /// <param name="properties">The properties to map.</param>
        /// <returns>A list of properties and their types.</returns>
        private static IEnumerable<Tuple<PropertyInfo, Type>> MapProperties(IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(p => new Tuple<PropertyInfo, Type>(p, p.PropertyType.GetCollectionType() ?? p.PropertyType));
        }

        /// <summary>
        /// Executes the current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of properties that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult> FetchMappedResultAsync<TResult>(IEnumerable<PropertyInfo> map)
            where TResult : new()
        {
            return _transaction.ExecuteMapResultAsync<TResult>(Render(), MapProperties(map));
        }
        
        /// <summary>
        /// Executes the current command and performs a custom action
        /// to create the result type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="valueProvider">
        /// Delegate function to convert the result to the specified type.
        /// </param>
        /// <returns>Result of command.</returns>
        public Task<TResult> FetchCustomResultAsync<TResult>(CreateEntity<TResult> valueProvider)
        {
            return _transaction.ExecuteCustomAsync<TResult>(Render(), valueProvider);
        }

        /// <summary>
        /// Executes the current command and returns the first two columns of the result as
        /// a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>
        /// Result of command as a dictionary.
        /// </returns>
        public Task<IDictionary<TKey, TValue>> FetchRowKeyedDictionaryAsync<TKey, TValue>()
        {
            return _transaction.ExecuteRowKeyedDictionaryResultAsync<TKey, TValue>(Render());
        }

        /// <summary>
        /// Fetches the first row of command as a dictionary with the column names as keys
        /// and the result row values as values.
        /// </summary>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>Result of command as a dictionary.</returns>
        public Task<IDictionary<string, object>> FetchColumnKeyedDictionaryAsync(params string[] columns)
        {
            return _transaction.ExecuteColumnKeyedDictionaryResultAsync(Render(), columns);
        }

        /// <summary>
        /// Executes the current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected by the command.</returns>
        public Task<int> ExecuteAsync()
        {
            return _transaction.ExecuteAsync(Render());
        }

        #endregion

        private RenderContext Render()
        {
            var context = new RenderContext(Dialect);
            context.RenderDelimited(_statements, context.LineDelimiter);

            return context;
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<ISqlStatement> IEnumerable<ISqlStatement>.GetEnumerator() 
            => _statements.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _statements.GetEnumerator();
    }
}
