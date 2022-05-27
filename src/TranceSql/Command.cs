using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql
{
    /// <summary>
    /// <see cref="Command"/> is the main class in TranceSQL operations. It is a
    /// collection of <see cref="ISqlStatement"/> (i.e. <see cref="Select"/>,
    /// <see cref="Update"/>, <see cref="Insert"/>, etc.) and provides a variety of
    /// methods for executing the SQL statements defined for the command.
    /// </summary>
    public class Command : IEnumerable<ISqlStatement>
    {
        private readonly List<ISqlStatement> _statements = new();
        private readonly SqlCommandManager _manager;
        private readonly DeferContext? _deferContext;
        private readonly Dictionary<string, object?> _namedParameters = new();

        /// <summary>
        /// Gets the dialect this command is configured for.
        /// </summary>
        internal IDialect Dialect { get; }

        /// <summary>
        /// Gets or sets the name of the operation to be used for recording
        /// tracing information.
        /// </summary>
        public string? OperationName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="connection">The connection to use when rendering and executing the command.</param>
        public Command(Database connection)
        {
            _manager = connection.Manager;
            Dialect = connection.Dialect;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class.
        /// </summary>
        /// <param name="connection">The connection to use when rendering and executing the command.</param>
        /// <param name="operationName">Name of the operation to be used for recording tracing information.</param>
        public Command(Database connection, string operationName)
            : this(connection)
        {
            OperationName = operationName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class for executing deferred commands.
        /// If deferred execution is not being used, the <see cref="Command(Database)" /> constructor
        /// is a better choice.
        /// </summary>
        /// <param name="deferContext">The defer context for this command.</param>
        public Command(DeferContext deferContext)
            : this(deferContext.Database)
        {
            _deferContext = deferContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class for executing deferred commands.
        /// If deferred execution is not being used, the <see cref="Command(Database)" /> constructor
        /// is a better choice.
        /// </summary>
        /// <param name="deferContext">The defer context for this command.</param>
        /// <param name="operationName">Name of the operation to be used for recording tracing information.</param>
        public Command(DeferContext deferContext, string operationName)
            : this(deferContext)
        {
            OperationName = operationName;
        }

        /// <summary>
        /// Adds the specified statement to this command.
        /// </summary>
        /// <param name="statement">The statement to add.</param>
        public void Add(ISqlStatement statement) => _statements.Add(statement);

        /// <summary>
        /// Includes a new named parameter in the command. These can be used in
        /// <see cref="Raw"/> elements. For normal use cases, using a dynamic
        /// <see cref="Value"/> parameter is likely a better choice.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>A parameter instance for the parameter specified.</returns>
        public Parameter IncludeParameter(string name, object value)
        {
            if (value is ISqlElement)
            {
                throw new InvalidCommandException(
                    "Attempted to pass an instance of an ISqlElement as a value in query, SQL elements should not be passed as values.");
            }

            var result = new Parameter(name);
            _namedParameters.Add(result.GetRequiredName(), value);
            return result;
        }

        #region Execution

        #region Cached

        #region Fetch List

        /// <summary>
        /// Creates a delegate from current command and returns the result as 
        /// an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <returns>Delegate for specified command.</returns>
        public Func<CancellationToken, Task<IEnumerable<TResult>?>> FetchListCached<TResult>()
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteListResultAsync<TResult>(cached, c);
        }

        /// <summary>
        /// Creates a delegate from current command to return the result as an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="parameter">The parameter, should be of type TParameter.</param>
        /// <returns>Delegate for specified command.</returns>
        public Func<TParameter, CancellationToken, Task<IEnumerable<TResult>?>> FetchListCached<TResult, TParameter>(
            Parameter parameter)
        {
            var cached = new CachedContext(Render());
            return (p, c) =>
                _manager.ExecuteListResultAsync<TResult>(
                    cached.WithParameters(new Dictionary<string, object?> {{parameter.GetRequiredName(), p}}), c);
        }

        /// <summary>
        /// Creates a delegate from current command to return the result as an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <returns>Delegate for specified command.</returns>
        public Func<TParameter1, TParameter2, CancellationToken, Task<IEnumerable<TResult>?>> FetchListCached<TResult,
            TParameter1, TParameter2>(Parameter firstParameter, Parameter secondParameter)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, c) => _manager.ExecuteListResultAsync<TResult>(
                cached.WithParameters(new Dictionary<string, object?>
                    {{firstParameter.GetRequiredName(), p1}, {secondParameter.GetRequiredName(), p2}}), c);
        }

        /// <summary>
        /// Creates a delegate from current command to return the result as an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="thirdParameter">The third parameter, should be of type TParameter3.</param>
        /// <returns>Delegate for specified command.</returns>
        public Func<TParameter1, TParameter2, TParameter3, CancellationToken, Task<IEnumerable<TResult>?>>
            FetchListCached<TResult, TParameter1, TParameter2, TParameter3>(Parameter firstParameter,
                Parameter secondParameter, Parameter thirdParameter)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, p3, c) => _manager.ExecuteListResultAsync<TResult>(
                cached.WithParameters(new Dictionary<string, object?>
                {
                    {firstParameter.GetRequiredName(), p1}, {secondParameter.GetRequiredName(), p2},
                    {thirdParameter.GetRequiredName(), p3}
                }), c);
        }

        /// <summary>
        /// Creates a delegate from current command to return the result as an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter.</typeparam>
        /// <typeparam name="TParameter4">The type of the fourth parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="thirdParameter">The third parameter, should be of type TParameter3.</param>
        /// <param name="fourthParameter">The fourth parameter, should be of type TParameter4.</param>
        /// <returns>Delegate for specified command.</returns>
        public Func<TParameter1, TParameter2, TParameter3, TParameter4, CancellationToken, Task<IEnumerable<TResult>?>>
            FetchListCached<TResult, TParameter1, TParameter2, TParameter3, TParameter4>(Parameter firstParameter,
                Parameter secondParameter, Parameter thirdParameter, Parameter fourthParameter)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, p3, p4, c) => _manager.ExecuteListResultAsync<TResult>(cached.WithParameters(
                new Dictionary<string, object?>
                {
                    {firstParameter.GetRequiredName(), p1},
                    {secondParameter.GetRequiredName(), p2},
                    {thirdParameter.GetRequiredName(), p3},
                    {fourthParameter.GetRequiredName(), p4}
                }), c);
        }

        #endregion

        #region Fetch

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="defaultValue">Value to return if result is null or command returns no values.</param>
        /// <returns>Result of command.</returns>
        public Func<CancellationToken, Task<TResult?>> FetchCached<TResult>(TResult? defaultValue = default)
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteResultAsync(Render(), defaultValue, null, c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="parameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Delegate for specified command.</returns>
        public Func<TParameter, CancellationToken, Task<TResult?>> FetchCached<TResult, TParameter>(Parameter parameter,
            TResult? defaultValue = default)
        {
            var cached = new CachedContext(Render());
            return (p, c) =>
                _manager.ExecuteResultAsync(
                    cached.WithParameters(new Dictionary<string, object?>
                    {
                        {parameter.GetRequiredName(), p}
                    }), defaultValue, null,
                    c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Delegate for specified command.
        /// </returns>
        public Func<TParameter1, TParameter2, CancellationToken, Task<TResult?>>
            FetchCached<TResult, TParameter1, TParameter2>(Parameter firstParameter, Parameter secondParameter,
                TResult? defaultValue = default)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, c) =>
                _manager.ExecuteResultAsync(
                    cached.WithParameters(new Dictionary<string, object?>
                    {
                        {firstParameter.GetRequiredName(), p1},
                        {secondParameter.GetRequiredName(), p2}
                    }), defaultValue, null, c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="thirdParameter">The third parameter, should be of type TParameter3.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Delegate for specified command.
        /// </returns>
        public Func<TParameter1, TParameter2, TParameter3, CancellationToken, Task<TResult?>>
            FetchCached<TResult, TParameter1, TParameter2, TParameter3>(Parameter firstParameter,
                Parameter secondParameter, Parameter thirdParameter, TResult? defaultValue = default)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, p3, c) =>
                _manager.ExecuteResultAsync(
                    cached.WithParameters(new Dictionary<string, object?>
                    {
                        {firstParameter.GetRequiredName(), p1},
                        {secondParameter.GetRequiredName(), p2},
                        {thirdParameter.GetRequiredName(), p3}
                    }),
                    defaultValue, null, c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter.</typeparam>
        /// <typeparam name="TParameter4">The type of the fourth parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="thirdParameter">The third parameter, should be of type TParameter3.</param>
        /// <param name="fourthParameter">The fourth parameter, should be of type TParameter4.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Delegate for specified command.
        /// </returns>
        public Func<TParameter1, TParameter2, TParameter3, TParameter4, CancellationToken, Task<TResult?>>
            FetchCached<TResult, TParameter1, TParameter2, TParameter3, TParameter4>(Parameter firstParameter,
                Parameter secondParameter, Parameter thirdParameter, Parameter fourthParameter,
                TResult? defaultValue = default)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, p3, p4, c) => _manager.ExecuteResultAsync(
                cached.WithParameters(new Dictionary<string, object?>
                {
                    {firstParameter.GetRequiredName(), p1},
                    {secondParameter.GetRequiredName(), p2},
                    {thirdParameter.GetRequiredName(), p3},
                    {fourthParameter.GetRequiredName(), p4}
                }), defaultValue, null, c);
        }

        #endregion

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Func<CancellationToken, Task<TResult?>> FetchCached<TResult>(
            params Expression<Func<TResult, IEnumerable>>[] collections)
        {
            var cached = new CachedContext(Render());
            return (c) =>
                _manager.ExecuteResultAsync<TResult>(Render(), default, collections.Select(i => i.GetPropertyInfo()),
                    c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Func<CancellationToken, Task<TResult?>> FetchCached<TResult>(IEnumerable<PropertyInfo> collections)
        {
            if (!collections.All(p => p.PropertyType.ImplementsInterface<IEnumerable>()))
            {
                throw new ArgumentException("All properties must be collections", "collections");
            }

            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteResultAsync<TResult>(Render(), default, collections, c);
        }

        /// <summary>
        /// Creates a delegate from current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Func<CancellationToken, Task<TResult?>> FetchMappedResultCached<TResult>(
            params Expression<Func<TResult, object>>[] map)
            where TResult : new()
        {
            var mappedProperties = MapProperties(map);
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteMapResultAsync<TResult>(Render(), mappedProperties, c);
        }

        /// <summary>
        /// Creates a delegate from current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of properties that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Func<CancellationToken, Task<TResult?>> FetchMappedResultCached<TResult>(IEnumerable<PropertyInfo> map)
            where TResult : new()
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteMapResultAsync<TResult>(Render(), MapProperties(map), c);
        }

        /// <summary>
        /// Creates a delegate from current command and performs a custom action
        /// to create the result type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="valueProvider">
        /// Delegate function to convert the result to the specified type.
        /// </param>
        /// <returns>Result of command.</returns>
        public Func<CancellationToken, Task<TResult?>> FetchCustomResultCached<TResult>(
            CreateEntity<TResult> valueProvider)
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteCustomAsync(Render(), valueProvider, c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns the first two columns of the result as
        /// a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>
        /// Result of command as a dictionary.
        /// </returns>
        public Func<CancellationToken, Task<IDictionary<TKey, TValue?>?>> FetchRowKeyedDictionaryCached<TKey, TValue>()
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteRowKeyedDictionaryResultAsync<TKey, TValue>(Render(), c);
        }

        /// <summary>
        /// Fetches the first row of command as a dictionary with the column names as keys
        /// and the result row values as values.
        /// </summary>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>Result of command as a dictionary.</returns>
        public Func<CancellationToken, Task<IDictionary<string, object?>?>> FetchColumnKeyedDictionaryCached(
            params string[] columns)
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteColumnKeyedDictionaryResultAsync(Render(), columns, c);
        }

        #region Execute

        /// <summary>
        /// Creates a delegate from current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <returns>Delegate for specified command.</returns>
        public Func<CancellationToken, Task<int>> ExecuteCached()
        {
            var cached = new CachedContext(Render());
            return (c) => _manager.ExecuteAsync(Render(), c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <typeparam name="TParameter">The type of the first parameter.</typeparam>
        /// <param name="parameter">The first parameter, should be of type TParameter1.</param>
        /// <returns>Delegate for specified command.</returns>
        public Func<TParameter, CancellationToken, Task<int>> ExecuteCached<TParameter>(Parameter parameter)
        {
            var cached = new CachedContext(Render());
            return (p, c) =>
                _manager.ExecuteAsync(cached.WithParameters(new Dictionary<string, object?>
                {
                    {parameter.GetRequiredName(), p}
                }), c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <returns>
        /// Delegate for specified command.
        /// </returns>
        public Func<TParameter1, TParameter2, CancellationToken, Task<int>> ExecuteCached<TParameter1, TParameter2>(
            Parameter firstParameter, Parameter secondParameter)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, c) =>
                _manager.ExecuteAsync(
                    cached.WithParameters(new Dictionary<string, object?>
                    {
                        {firstParameter.GetRequiredName(), p1},
                        {secondParameter.GetRequiredName(), p2}
                    }), c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="thirdParameter">The third parameter, should be of type TParameter3.</param>
        /// <returns>
        /// Delegate for specified command.
        /// </returns>
        public Func<TParameter1, TParameter2, TParameter3, CancellationToken, Task<int>>
            ExecuteCached<TParameter1, TParameter2, TParameter3>(Parameter firstParameter, Parameter secondParameter,
                Parameter thirdParameter)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, p3, c) =>
                _manager.ExecuteAsync(
                    cached.WithParameters(new Dictionary<string, object?>
                    {
                        {firstParameter.GetRequiredName(), p1},
                        {secondParameter.GetRequiredName(), p2},
                        {thirdParameter.GetRequiredName(), p3}
                    }), c);
        }

        /// <summary>
        /// Creates a delegate from current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter.</typeparam>
        /// <typeparam name="TParameter4">The type of the fourth parameter.</typeparam>
        /// <param name="firstParameter">The first parameter, should be of type TParameter1.</param>
        /// <param name="secondParameter">The second parameter, should be of type TParameter2.</param>
        /// <param name="thirdParameter">The third parameter, should be of type TParameter3.</param>
        /// <param name="fourthParameter">The fourth parameter, should be of type TParameter4.</param>
        /// <returns>
        /// Delegate for specified command.
        /// </returns>
        public Func<TParameter1, TParameter2, TParameter3, TParameter4, CancellationToken, Task<int>>
            ExecuteCached<TParameter1, TParameter2, TParameter3, TParameter4>(Parameter firstParameter,
                Parameter secondParameter, Parameter thirdParameter, Parameter fourthParameter)
        {
            var cached = new CachedContext(Render());
            return (p1, p2, p3, p4, c) =>
                _manager.ExecuteAsync(
                    cached.WithParameters(new Dictionary<string, object?>
                    {
                        {firstParameter.GetRequiredName(), p1},
                        {secondParameter.GetRequiredName(), p2},
                        {thirdParameter.GetRequiredName(), p3},
                        {fourthParameter.GetRequiredName(), p4}
                    }), c);
        }

        #endregion

        #endregion

        #region Synchronous

        /// <summary>
        /// Executes the current command and returns the result as
        /// an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <returns>
        /// Result of command as a list.
        /// </returns>
        public IEnumerable<TResult?>? FetchList<TResult>()
        {
            return _manager.ExecuteListResult<TResult>(Render());
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="defaultValue">Value to return if result is null or command returns no values.</param>
        /// <returns>Result of command.</returns>
        public TResult? Fetch<TResult>(TResult? defaultValue = default)
        {
            return _manager.ExecuteResult(Render(), defaultValue, null);
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
        public TResult? Fetch<TResult>(params Expression<Func<TResult, IEnumerable>>[] collections)
        {
            return _manager.ExecuteResult<TResult>(Render(), default, collections.Select(i => i.GetPropertyInfo()));
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
        public TResult? Fetch<TResult>(IEnumerable<PropertyInfo> collections)
        {
            if (!collections.All(p => p.PropertyType.ImplementsInterface<IEnumerable>()))
            {
                throw new ArgumentException("All properties must be collections", nameof(collections));
            }

            return _manager.ExecuteResult<TResult>(Render(), default, collections);
        }

        /// <summary>
        /// Executes the current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>
        /// The result of the SQL command.
        /// </returns>
        public TResult? FetchMappedResult<TResult>(params Expression<Func<TResult, object>>[] map)
            where TResult : new()
        {
            var mappedProperties = MapProperties(map);
            return _manager.ExecuteMapResult<TResult>(Render(), mappedProperties);
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
        public TResult? FetchMappedResult<TResult>(IEnumerable<PropertyInfo> map)
            where TResult : new()
        {
            return _manager.ExecuteMapResult<TResult>(Render(), MapProperties(map));
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
        public TResult? FetchCustomResult<TResult>(CreateEntity<TResult> valueProvider)
        {
            return _manager.ExecuteCustom(Render(), valueProvider);
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
        public IDictionary<TKey, TValue?>? FetchRowKeyedDictionary<TKey, TValue>()
        {
            return _manager.ExecuteRowKeyedDictionaryResult<TKey, TValue?>(Render());
        }

        /// <summary>
        /// Fetches the first row of command as a dictionary with the column names as keys
        /// and the result row values as values.
        /// </summary>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>Result of command as a dictionary.</returns>
        public IDictionary<string, object?>? FetchColumnKeyedDictionary(params string[] columns)
        {
            return _manager.ExecuteColumnKeyedDictionaryResult(Render(), columns);
        }

        /// <summary>
        /// Executes the current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected by the command.</returns>
        public int Execute()
        {
            return _manager.Execute(Render());
        }

        #endregion

        #region Async

        /// <summary>
        /// Executes the current command and returns the result as 
        /// an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>Result of command as a list.</returns>
        public Task<IEnumerable<TResult>?> FetchListAsync<TResult>(CancellationToken cancel = default)
        {
            return _manager.ExecuteListResultAsync<TResult>(Render(), cancel);
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="defaultValue">Value to return if result is null or command returns no values.</param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>Result of command.</returns>
        public Task<TResult?> FetchAsync<TResult>(TResult? defaultValue = default, CancellationToken cancel = default)
        {
            return _manager.ExecuteResultAsync(Render(), defaultValue, null, cancel);
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult?> FetchAsync<TResult>(CancellationToken cancel = default,
            params Expression<Func<TResult, IEnumerable>>[] collections)
        {
            return _manager.ExecuteResultAsync<TResult>(Render(), default, collections.Select(i => i.GetPropertyInfo()),
                cancel);
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
        public Task<TResult?> FetchAsync<TResult>(params Expression<Func<TResult, IEnumerable>>[] collections)
        {
            return _manager.ExecuteResultAsync<TResult>(Render(), default, collections.Select(i => i.GetPropertyInfo()),
                default);
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult?> FetchAsync<TResult>(IEnumerable<PropertyInfo> collections,
            CancellationToken cancel = default)
        {
            if (!collections.All(p => p.PropertyType.ImplementsInterface<IEnumerable>()))
            {
                throw new ArgumentException("All properties must be collections", "collections");
            }

            return _manager.ExecuteResultAsync<TResult>(Render(), default, collections, cancel);
        }

        /// <summary>
        /// Executes the current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult?> FetchMappedResultAsync<TResult>(CancellationToken cancel,
            params Expression<Func<TResult, object>>[] map)
            where TResult : new()
        {
            var mappedProperties = MapProperties(map);
            return _manager.ExecuteMapResultAsync<TResult>(Render(), mappedProperties, cancel);
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
        public Task<TResult?> FetchMappedResultAsync<TResult>(params Expression<Func<TResult, object>>[] map)
            where TResult : new()
        {
            var mappedProperties = MapProperties(map);
            return _manager.ExecuteMapResultAsync<TResult>(Render(), mappedProperties, default);
        }

        /// <summary>
        /// Maps a list of properties expression to properties and types.
        /// </summary>
        /// <typeparam name="TResult">The entity type to select.</typeparam>
        /// <param name="map">A list of property select expressions.</param>
        /// <returns>
        /// A list of properties and their types.
        /// </returns>
        private static IEnumerable<Tuple<PropertyInfo, Type>> MapProperties<TResult>(
            IEnumerable<Expression<Func<TResult, object>>> map)
        {
            return MapProperties(map.Select(i => i.GetPropertyInfo()));
        }

        /// <summary>
        /// Maps a list of properties to their property types.
        /// </summary>
        /// <param name="properties">The properties to map.</param>
        /// <returns>
        /// A list of properties and their types.
        /// </returns>
        private static IEnumerable<Tuple<PropertyInfo, Type>> MapProperties(IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(p =>
                new Tuple<PropertyInfo, Type>(p, p.PropertyType.GetCollectionType() ?? p.PropertyType));
        }

        /// <summary>
        /// Executes the current command and maps multiple commands to a single result class. Use
        /// this method to populate a result with multiple commands.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="map">A list of properties that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <returns>
        /// Result of command.
        /// </returns>
        public Task<TResult?> FetchMappedResultAsync<TResult>(IEnumerable<PropertyInfo> map,
            CancellationToken cancel = default)
            where TResult : new()
        {
            return _manager.ExecuteMapResultAsync<TResult>(Render(), MapProperties(map), cancel);
        }

        /// <summary>
        /// Executes the current command and performs a custom action
        /// to create the result type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="valueProvider">
        /// Delegate function to convert the result to the specified type.
        /// </param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>Result of command.</returns>
        public Task<TResult?> FetchCustomResultAsync<TResult>(CreateEntity<TResult> valueProvider,
            CancellationToken cancel = default)
        {
            return _manager.ExecuteCustomAsync(Render(), valueProvider, cancel);
        }

        /// <summary>
        /// Executes the current command and returns the first two columns of the result as
        /// a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// Result of command as a dictionary.
        /// </returns>
        public Task<IDictionary<TKey, TValue?>?> FetchRowKeyedDictionaryAsync<TKey, TValue>(
            CancellationToken cancel = default)
        {
            return _manager.ExecuteRowKeyedDictionaryResultAsync<TKey, TValue>(Render(), cancel);
        }

        /// <summary>
        /// Fetches the first row of command as a dictionary with the column names as keys
        /// and the result row values as values.
        /// </summary>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>Result of command as a dictionary.</returns>
        public Task<IDictionary<string, object?>?> FetchColumnKeyedDictionaryAsync(CancellationToken cancel = default,
            params string[] columns)
        {
            return _manager.ExecuteColumnKeyedDictionaryResultAsync(Render(), columns, cancel);
        }

        /// <summary>
        /// Fetches the first row of command as a dictionary with the column names as keys
        /// and the result row values as values.
        /// </summary>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>Result of command as a dictionary.</returns>
        public Task<IDictionary<string, object?>?> FetchColumnKeyedDictionaryAsync(params string[] columns)
        {
            return _manager.ExecuteColumnKeyedDictionaryResultAsync(Render(), columns, default);
        }

        /// <summary>
        /// Executes the current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <param name="cancel">A token to monitor for cancellation requests.</param>
        /// <returns>The number of rows affected by the command.</returns>
        public Task<int> ExecuteAsync(CancellationToken cancel = default)
        {
            return _manager.ExecuteAsync(Render(), cancel);
        }

        #endregion

        #region Deferred

        /// <summary>
        /// Executes the current command and returns the result as 
        /// an enumerable list.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <returns>Result of command as a list.</returns>
        public Deferred<IEnumerable<TResult?>> FetchListDeferred<TResult>()
        {
            return GetAvailableDeferred().ExecuteListResultDeferred<TResult?>(Render());
        }

        /// <summary>
        /// Executes the current command and returns a single row as
        /// the specified type.
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <param name="defaultValue">Value to return if result is null or command returns no values.</param>
        /// <returns>Result of command.</returns>
        public Deferred<TResult?> FetchDeferred<TResult>(TResult? defaultValue = default)
        {
            return GetAvailableDeferred().ExecuteResultDeferred<TResult?>(Render(), defaultValue, null);
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
        public Deferred<TResult> FetchDeferred<TResult>(params Expression<Func<TResult, object>>[] collections)
        {
            return GetAvailableDeferred().ExecuteResultDeferred<TResult>(Render(), default(TResult),
                collections.Select(i => i.GetPropertyInfo()));
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
        public Deferred<TResult> FetchDeferred<TResult>(IEnumerable<PropertyInfo> collections)
        {
            if (!collections.All(p => p.PropertyType.ImplementsInterface<IEnumerable>()))
            {
                throw new ArgumentException("All properties must be collections", nameof(collections));
            }

            return GetAvailableDeferred().ExecuteResultDeferred<TResult>(Render(), default(TResult), collections);
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
        public Deferred<TResult> FetchMappedResultDeferred<TResult>(params Expression<Func<TResult, object>>[] map)
            where TResult : new()
        {
            var mappedProperties = MapProperties(map);
            return GetAvailableDeferred().ExecuteMapResultDeferred<TResult>(Render(), mappedProperties);
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
        public Deferred<TResult> FetchMappedResultDeferred<TResult>(IEnumerable<PropertyInfo> map)
            where TResult : new()
        {
            return GetAvailableDeferred().ExecuteMapResultDeferred<TResult>(Render(), MapProperties(map));
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
        public Deferred<TResult> FetchCustomResultDeferred<TResult>(CreateEntity<TResult> valueProvider)
        {
            return GetAvailableDeferred().ExecuteCustomDeferred(Render(), valueProvider);
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
        public Deferred<IDictionary<TKey, TValue>> FetchRowKeyedDictionaryDeferred<TKey, TValue>()
        {
            return GetAvailableDeferred().ExecuteRowKeyedDictionaryResultDeferred<TKey, TValue>(Render());
        }

        /// <summary>
        /// Fetches the first row of command as a dictionary with the column names as keys
        /// and the result row values as values.
        /// </summary>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>Result of command as a dictionary.</returns>
        public Deferred<IDictionary<string, object>> FetchColumnKeyedDictionaryDeferred(params string[] columns)
        {
            return GetAvailableDeferred().ExecuteColumnKeyedDictionaryResultDeferred(Render(), columns);
        }

        /// <summary>
        /// Ensures the result of the deferred context's operation is available, otherwise
        /// throws an exception.
        /// </summary>
        private DeferContext GetAvailableDeferred()
        {
            if (_deferContext is null)
            {
                throw new InvalidOperationException(
                    "No deferred command context exists for this command. To execute a deferred command you must provide a context when the command instance is created.");
            }

            if (_deferContext.HasExecuted)
            {
                throw new InvalidOperationException(
                    "The deferred context for this command has already executed. To execute additional deferred commands you must create a new deferred context.");
            }

            return _deferContext;
        }

        /// <summary>
        /// Executes the current command and returns a count of the
        /// number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected by the command.</returns>
        public void ExecuteDeferred()
        {
            GetAvailableDeferred().ExecuteDeferred(Render());
        }

        #endregion

        /// <summary>
        /// Creates a streaming enumerable which when executed will execute
        /// the command and provide the results as the items of an enumeration.
        /// This can be helpful for enumerating over large results sets without
        /// needing to place the entire result set in memory or waiting for all'
        /// results before execution can begin. Note that each enumeration of the
        /// result will execute the current command!
        /// </summary>
        /// <typeparam name="TResult">Result item type</typeparam>
        /// <returns>
        /// Result of command as a list.
        /// </returns>
        public IEnumerable<TResult> FetchStream<TResult>()
        {
            // use a cached context here since execution may be deferred
            var cached = new CachedContext(Render());
            return _manager.ExecuteStream<TResult>(cached);
        }

        #endregion

        private RenderContext Render()
        {
            var context = new RenderContext(Dialect, _deferContext)
            {
                OperationName = OperationName
            };

            context.NamedParameters = _namedParameters;

            context.RenderDelimited(_statements, context.LineDelimiter);

            return context;
        }

        #region IEnumerable

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

        #endregion

        /// <summary>Converts to string.</summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            var context = new RenderContext(Dialect);
            context.RenderDelimited(_statements, context.LineDelimiter);
            return context.CommandText;
        }
    }
}