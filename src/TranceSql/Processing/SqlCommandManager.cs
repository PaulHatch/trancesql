using OpenTracing;
using OpenTracing.Tag;
using OpenTracing.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql.Processing
{
    /// <summary>Handler for ADO interactions.</summary>
    public class SqlCommandManager
    {
        /// <summary>Connection factory delegate for target database.</summary>
        internal Func<DbConnection> ConnectionFactory { get; }

        /// <summary>Provides parameter value from input object instances.</summary>
        internal IParameterValueExtractor ValueExtractor { get; }

        private enum ConnectionMode { String, AsyncFactory, Factory }

        private DbInfo _dbInfo;
        private volatile string _connectionString;
        private readonly ITracer _tracer;
        private readonly RollingCredentials _rollingCredentials;
        private readonly ConnectionMode _connectionMode = ConnectionMode.String;
        private readonly Func<string, DbInfo> _dbInfoFactory;

        /// <summary>
        /// Initialize a new instance of a generic ADO transaction. Use
        /// a provider-specific subclass for better performance.
        /// </summary>
        /// <param name="rollingCredentials">A connection string provider which uses rolling credentials.</param>
        /// <param name="connectionFactory">Delegate to create connections for current database.</param>
        /// <param name="valueExtractor">Provides parameter value from input object instances.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        /// <param name="dbInfoFactory">The information about the connection string being used.</param>
        public SqlCommandManager(
            RollingCredentials rollingCredentials,
            Func<DbConnection> connectionFactory,
            IParameterValueExtractor valueExtractor,
            ITracer tracer,
            Func<string, DbInfo> dbInfoFactory)
            : this((string)null, connectionFactory, valueExtractor, tracer, null)
        {
            _rollingCredentials = rollingCredentials;
            _connectionMode = ConnectionMode.AsyncFactory;
            _dbInfoFactory = dbInfoFactory;
        }

        /// <summary>
        /// Initialize a new instance of a generic ADO transaction. Use
        /// a provider-specific subclass for better performance.
        /// </summary>
        /// <param name="connectionString">Connection string to target database.</param>
        /// <param name="connectionFactory">Delegate to create connections for current database.</param>
        /// <param name="valueExtractor">Provides parameter value from input object instances.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        /// <param name="dbInfo">The information about the connection string being used.</param>
        public SqlCommandManager(
            string connectionString,
            Func<DbConnection> connectionFactory,
            IParameterValueExtractor valueExtractor,
            ITracer tracer,
            DbInfo dbInfo)
        {
            _connectionString = connectionString;
            ConnectionFactory = connectionFactory;
            ValueExtractor = valueExtractor;
            _tracer = tracer ?? GlobalTracer.Instance;
            _dbInfo = dbInfo;
        }

        /// <summary>
        /// Adds the parameters in this transaction to the specified command.
        /// </summary>
        /// <param name="command">Target DbCommand instance to add parameters to.</param>
        /// <param name="context">The SQL context to run.</param>
        internal void AddParametersToCommand(DbCommand command, IContext context)
        {
            // Add static parameters to command.
            foreach (var parameter in context.ParameterValues)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = parameter.Key;
                ValueExtractor.SetValue(sqlParam, parameter.Value);
                command.Parameters.Add(sqlParam);
            }
        }

        /// <summary>
        /// Creates a connection for this transaction.
        /// </summary>
        /// <param name="forceRefresh">True if refreshing credentials must be refreshed.</param>
        /// <returns>
        /// A DbConnection instance for this transaction's target database.
        /// </returns>
        internal async Task<DbConnection> CreateConnectionAsync(bool forceRefresh = false)
        {
            var newConnection = ConnectionFactory();

            if (_connectionMode == ConnectionMode.String)
            {
                newConnection.ConnectionString = _connectionString;
            }
            else
            {
                newConnection.ConnectionString = await _rollingCredentials.GetConnectionStringAsync();
            }

            return newConnection;
        }

        #region Async Execution

        /// <summary>
        /// Executes the specified SQL command text and binds the rows selected
        /// to an enumerable list of the specified type.
        /// </summary>
        /// <typeparam name="T">Result element type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <returns>The result of the SQL command.</returns>
        internal Task<IEnumerable<T>> ExecuteListResultAsync<T>(IContext context)
        {
            var processor = new ListResultProcessor<T>();
            return RunCommandAsync<IEnumerable<T>>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and binds the first row
        /// selected to the specified type. If no rows are returned or if the
        /// value of a simple type is null, the value specified by the default
        /// result parameter will be returned instead.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="defaultResult">Value to return in case no data is present.</param>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <exception cref="System.InvalidOperationException">Each collection argument must have a corresponding select command.</exception>
        internal Task<T> ExecuteResultAsync<T>(IContext context, T defaultResult, IEnumerable<PropertyInfo> collections)
        {
            var processor = new SingleResultProcessor<T>(defaultResult, collections);
            return RunCommandAsync<T>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and maps the results to the
        /// specified type.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="map">The map.</param>
        /// <returns>
        /// The result of the SQL command.
        /// </returns>
        internal Task<T> ExecuteMapResultAsync<T>(IContext context, IEnumerable<Tuple<PropertyInfo, Type>> map)
            where T : new()
        {
            var processor = new MappedResultProcessor<T>(map);
            return RunCommandAsync<T>(context, processor);
        }

        /// <summary>
        /// Executes a non-query SQL command and returns the number of rows affected.
        /// </summary>
        /// <param name="context">The SQL context to run.</param>
        /// <returns>The number of rows affected.</returns>
        internal Task<int> ExecuteAsync(IContext context)
        {
            return RunCommandAsync<int>(context, null);
        }

        /// <summary>
        /// Executes the specified SQL command text and runs a custom delegate
        /// to process the results.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="valueProvider">Custom delegate to create the result.</param>
        /// <returns>The result of the SQL command.</returns>
        internal Task<T> ExecuteCustomAsync<T>(IContext context, CreateEntity<T> valueProvider)
        {
            var processor = new CustomResultProcessor<T>(valueProvider);
            return RunCommandAsync<T>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and bind the selected rows to
        /// a dictionary from the first two columns of the results.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The key type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <returns>The result of the SQL command.</returns>
        internal Task<IDictionary<TKey, TValue>> ExecuteRowKeyedDictionaryResultAsync<TKey, TValue>(IContext context)
        {
            var processor = new RowKeyedDictionaryResultProcessor<TKey, TValue>();
            return RunCommandAsync<IDictionary<TKey, TValue>>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and bind the selected row to
        /// a dictionary from the columns names and values of the result.
        /// </summary>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>The result of the SQL command.</returns>
        internal Task<IDictionary<string, object>> ExecuteColumnKeyedDictionaryResultAsync(IContext context, IEnumerable<string> columns)
        {
            var processor = new ColumnKeyedDictionaryResultProcessor(columns);
            return RunCommandAsync<IDictionary<string, object>>(context, processor);
        }

        #endregion

        #region Normal Execution

        /// <summary>
        /// Executes the specified SQL command text and binds the rows selected
        /// to an enumerable list of the specified type.
        /// </summary>
        /// <typeparam name="T">Result element type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <returns>The result of the SQL command.</returns>
        internal IEnumerable<T> ExecuteListResult<T>(IContext context)
        {
            var processor = new ListResultProcessor<T>();
            return RunCommand<IEnumerable<T>>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and binds the first row
        /// selected to the specified type. If no rows are returned or if the
        /// value of a simple type is null, the value specified by the default
        /// result parameter will be returned instead.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="defaultResult">Value to return in case no data is present.</param>
        /// <param name="collections">A list of IEnumerable property selectors that should be populated from the command.
        /// These properties should appear in the same order as their select command.</param>
        /// <returns>The result of the SQL command.</returns>
        /// <exception cref="System.InvalidOperationException">Each collection argument must have a corresponding select command.</exception>
        internal T ExecuteResult<T>(IContext context, T defaultResult, IEnumerable<PropertyInfo> collections)
        {
            var processor = new SingleResultProcessor<T>(defaultResult, collections);
            return RunCommand<T>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and maps the results to the
        /// specified type.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="map">The map.</param>
        /// <returns>
        /// The result of the SQL command.
        /// </returns>
        internal T ExecuteMapResult<T>(IContext context, IEnumerable<Tuple<PropertyInfo, Type>> map)
            where T : new()
        {
            var processor = new MappedResultProcessor<T>(map);
            return RunCommand<T>(context, processor);
        }

        /// <summary>
        /// Executes a non-query SQL command and returns the number of rows affected.
        /// </summary>
        /// <param name="context">The SQL context to run.</param>
        /// <returns>The number of rows affected.</returns>
        internal int Execute(IContext context)
        {
            return RunCommand<int>(context, null);
        }

        /// <summary>
        /// Executes the specified SQL command text and runs a custom delegate
        /// to process the results.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="valueProvider">Custom delegate to create the result.</param>
        /// <returns>The result of the SQL command.</returns>
        internal T ExecuteCustom<T>(IContext context, CreateEntity<T> valueProvider)
        {
            var processor = new CustomResultProcessor<T>(valueProvider);
            return RunCommand<T>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and bind the selected rows to
        /// a dictionary from the first two columns of the results.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The key type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <returns>The result of the SQL command.</returns>
        internal IDictionary<TKey, TValue> ExecuteRowKeyedDictionaryResult<TKey, TValue>(IContext context)
        {
            var processor = new RowKeyedDictionaryResultProcessor<TKey, TValue>();
            return RunCommand<IDictionary<TKey, TValue>>(context, processor);
        }

        /// <summary>
        /// Executes the specified SQL command text and binds the selected row to
        /// a dictionary from the columns names and values of the result.
        /// </summary>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
        /// <returns>The result of the SQL command.</returns>
        internal IDictionary<string, object> ExecuteColumnKeyedDictionaryResult(IContext context, IEnumerable<string> columns)
        {
            var processor = new ColumnKeyedDictionaryResultProcessor(columns);
            return RunCommand<IDictionary<string, object>>(context, processor);
        }

        #endregion

        /// <summary>
        /// Returns a stream which when enumerated will execute the specified SQL command text
        /// and bind the results as an enumeration.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">A SQL command context which includes a SELECT command.</param>
        /// <returns>The result of the SQL command.</returns>
        internal IEnumerable<T> ExecuteStream<T>(IContext context)
        {
            return new ResultStream<T>(context, this);
        }

        private IScope CreateScope(IContext context)
        {
            var builder = _tracer
                .BuildSpan(context.OperationName ?? "sql-command")
                .WithTag(Tags.SpanKind.Key, Tags.SpanKindClient)
                .WithTag(Tags.DbType, "sql")
                .WithTag(Tags.Component.Key, "trancesql");

            if (!String.IsNullOrEmpty(_dbInfo.User)) { builder.WithTag(Tags.DbUser, _dbInfo.User); }
            if (!String.IsNullOrEmpty(_dbInfo.Database)) { builder.WithTag(Tags.DbInstance, _dbInfo.Database); }
            if (!String.IsNullOrEmpty(context.CommandText)) { builder.WithTag(Tags.DbStatement, context.CommandText); }
            if (!String.IsNullOrEmpty(_dbInfo.Server)) { builder.WithTag("peer.address", _dbInfo.Server); }

            return builder.StartActive(finishSpanOnDispose: true);
        }

        /// <summary>
        /// Runs the specified SQL as an asynchronous operation.
        /// </summary>
        /// <param name="context">The SQL context to run.</param>
        /// <param name="processors">The processors to use for the result sets.</param>
        /// <returns>A task for the operation.</returns>
        internal void RunCommandSet(IContext context, IEnumerable<ProcessorContext> processors)
        {
            using (var connection = AsyncHelper.RunSync(() => CreateConnectionAsync()))
            {
                using (var command = connection.CreateCommand())
                {
                    // initialize command
                    command.Connection = connection;
                    command.CommandText = context.CommandText;
                    AddParametersToCommand(command, context);

                    using (IScope scope = CreateScope(context))
                    {
                        try
                        {
                            connection.Open();
                            if (processors?.Any() != true)
                            {
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    var results = 0;
                                    foreach (var item in processors)
                                    {
                                        if (results > 0 && !reader.NextResult())
                                        {
                                            throw new InvalidOperationException($"Expected {processors.Count()} but result only contained {results}");
                                        }
                                        item.Deferred.SetValue(item.Processer.Process(reader));
                                        results++;
                                    }
                                }
                            }
                            connection.Close();
                        }
                        catch
                        {
                            scope.Span.SetTag(Tags.Error, true);
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Runs the specified SQL as an asynchronous operation.
        /// </summary>
        /// <param name="context">The SQL context to run.</param>
        /// <param name="processors">The processors to use for the result sets.</param>
        /// <returns>A task for the operation.</returns>
        internal async Task RunCommandSetAsync(IContext context, IEnumerable<ProcessorContext> processors)
        {
            using (var connection = await CreateConnectionAsync())
            {
                using (var command = connection.CreateCommand())
                {
                    // initialize command
                    command.Connection = connection;
                    command.CommandText = context.CommandText;
                    AddParametersToCommand(command, context);

                    using (IScope scope = CreateScope(context))
                    {
                        try
                        {
                            await connection.OpenAsync();
                            if (processors?.Any() != true)
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var results = 0;
                                    foreach (var item in processors)
                                    {
                                        if (results > 0 && !await reader.NextResultAsync())
                                        {
                                            throw new InvalidOperationException($"Expected {processors.Count()} but result only contained {results}");
                                        }
                                        item.Deferred.SetValue(item.Processer.Process(reader));
                                        results++;
                                    }
                                }
                            }
                            connection.Close();
                        }
                        catch
                        {
                            scope.Span.SetTag(Tags.Error, true);
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Run the specified SQL command as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The expected result type.</typeparam>
        /// <param name="context">The SQL context to run.</param>
        /// <param name="processor">
        /// The query processor. If this value is null, a non-query type command
        /// is assumed and the integer type will be assumed.
        /// </param>
        /// <returns>An asynchronous task for the query result.</returns>
        private async Task<T> RunCommandAsync<T>(IContext context, IResultProcessor processor = null)
        {
            AssertCorrectReturnType<T>(processor);

            using (var connection = await CreateConnectionAsync())
            {
                using (var command = connection.CreateCommand())
                {
                    // initialize command
                    command.Connection = connection;
                    command.CommandText = context.CommandText;
                    AddParametersToCommand(command, context);

                    object result;

                    using (IScope scope = CreateScope(context))
                    {
                        try
                        {
                            await connection.OpenAsync();
                            if (processor == null)
                            {
                                result = await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    result = processor.Process(reader);
                                }
                            }
                            connection.Close();
                        }
                        catch
                        {
                            scope.Span.SetTag(Tags.Error, true);
                            throw;
                        }
                    }

                    return (T)result;
                }
            }
        }

        /// <summary>
        /// Run the specified SQL command as an normal, thread-blocking operation.
        /// </summary>
        /// <typeparam name="T">The expected result type.</typeparam>
        /// <param name="context">The SQL context to run.</param>
        /// <param name="processor">
        /// The query processor. If this value is null, a non-query type command
        /// is assumed and the integer type will be assumed.
        /// </param>
        /// <returns>An asynchronous task for the query result.</returns>
        private T RunCommand<T>(IContext context, IResultProcessor processor = null)
        {
            AssertCorrectReturnType<T>(processor);

            using (var connection = AsyncHelper.RunSync(() => CreateConnectionAsync()))
            {
                using (var command = connection.CreateCommand())
                {
                    // initialize command
                    command.Connection = connection;
                    command.CommandText = context.CommandText;
                    AddParametersToCommand(command, context);

                    object result;

                    using (IScope scope = CreateScope(context))
                    {
                        try
                        {
                            connection.Open();
                            if (processor == null)
                            {
                                result = command.ExecuteNonQuery();
                            }
                            else
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    result = processor.Process(reader);
                                }
                            }
                            connection.Close();
                        }
                        catch
                        {
                            scope.Span.SetTag(Tags.Error, true);
                            throw;
                        }
                    }

                    return (T)result;
                }
            }
        }

        private static void AssertCorrectReturnType<T>(IResultProcessor processor)
        {
            if (processor == null && typeof(T) != typeof(int))
            {
                throw new ArgumentException($"Attempted to run a non-query command with the return type '{typeof(T).FullName}'. Non-query commands must return the type 'int'.", "T");
            }
        }
    }
}

