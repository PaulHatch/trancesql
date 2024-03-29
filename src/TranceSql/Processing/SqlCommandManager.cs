﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TranceSql.Processing;

/// <summary>Handler for ADO interactions.</summary>
public class SqlCommandManager
{
    /// <summary>Provides parameter value from input object instances.</summary>
    internal IParameterMapper ParameterMapper { get; }

    private readonly ActivitySource? _activitySource;
    private readonly IConnectionFactory _connectionFactory;

    /// <summary>
    /// Initialize a new instance of a generic ADO transaction. Use
    /// a provider-specific subclass for better performance.
    /// </summary>
    /// <param name="connectionFactory">Factory to create connections for current database.</param>
    /// <param name="parameterMapper">Provides parameter value from input object instances.</param>
    /// <param name="activitySource">The OpenTelemetry ActivitySource instance to use. If this value is null the global tracer will
    /// be used instead.</param>
    public SqlCommandManager(
        IConnectionFactory connectionFactory,
        IParameterMapper parameterMapper,
        ActivitySource? activitySource)
    {
        _connectionFactory = connectionFactory;
        ParameterMapper = parameterMapper;
        _activitySource = activitySource;
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
            ParameterMapper.SetValue(sqlParam, parameter.Value);
            command.Parameters.Add(sqlParam);
        }
    }

    internal Task<DbConnection> CreateConnectionAsync() => Task.FromResult(_connectionFactory.CreateConnection());


    #region Async Execution

    /// <summary>
    /// Executes the specified SQL command text and binds the rows selected
    /// to an enumerable list of the specified type.
    /// </summary>
    /// <typeparam name="T">Result element type.</typeparam>
    /// <param name="context">A SQL command context which includes a SELECT command.</param>
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>The result of the SQL command.</returns>
    internal async Task<IEnumerable<T>?> ExecuteListResultAsync<T>(IContext context, CancellationToken cancel)
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new ListResultProcessor<T>();
        return await RunCommandAsync<IEnumerable<T>>(context, processor, cancel);
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
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <exception cref="System.InvalidOperationException">Each collection argument must have a corresponding select command.</exception>
    internal async Task<T?> ExecuteResultAsync<T>(IContext context, T? defaultResult,
        IEnumerable<PropertyInfo>? collections, CancellationToken cancel)
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new SingleResultProcessor<T>(defaultResult, collections);
        return await RunCommandAsync<T>(context, processor, cancel);
    }

    /// <summary>
    /// Executes the specified SQL command text and maps the results to the
    /// specified type.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="context">A SQL command context which includes a SELECT command.</param>
    /// <param name="map">The map.</param>
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The result of the SQL command.
    /// </returns>
    internal async Task<T?> ExecuteMapResultAsync<T>(IContext context, IEnumerable<Tuple<PropertyInfo, Type>> map,
        CancellationToken cancel)
        where T : new()
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new MappedResultProcessor<T>(map);
        return await RunCommandAsync<T>(context, processor, cancel);
    }

    /// <summary>
    /// Executes a non-query SQL command and returns the number of rows affected.
    /// </summary>
    /// <param name="context">The SQL context to run.</param>
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>The number of rows affected.</returns>
    internal async Task<int> ExecuteAsync(IContext context, CancellationToken cancel)
    {
        if (context.CommandText.Length == 0) return default;
            
        return await RunCommandAsync<int>(context, null, cancel);
    }

    /// <summary>
    /// Executes the specified SQL command text and runs a custom delegate
    /// to process the results.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="context">A SQL command context which includes a SELECT command.</param>
    /// <param name="valueProvider">Custom delegate to create the result.</param>
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>The result of the SQL command.</returns>
    internal async Task<T?> ExecuteCustomAsync<T>(IContext context, CreateEntity<T> valueProvider,
        CancellationToken cancel)
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new CustomResultProcessor<T>(valueProvider);
        return await RunCommandAsync<T>(context, processor, cancel);
    }

    /// <summary>
    /// Executes the specified SQL command text and bind the selected rows to
    /// a dictionary from the first two columns of the results.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The key type.</typeparam>
    /// <param name="context">A SQL command context which includes a SELECT command.</param>
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>The result of the SQL command.</returns>
    internal async Task<IDictionary<TKey, TValue?>?> ExecuteRowKeyedDictionaryResultAsync<TKey, TValue>(
        IContext context,
        CancellationToken cancel)
        where TKey : notnull
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new RowKeyedDictionaryResultProcessor<TKey, TValue>();
        var result = await RunCommandAsync<IDictionary<TKey, TValue?>>(context, processor, cancel)
            .ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Executes the specified SQL command text and bind the selected row to
    /// a dictionary from the columns names and values of the result.
    /// </summary>
    /// <param name="context">A SQL command context which includes a SELECT command.</param>
    /// <param name="columns">The columns to return. If null, all columns will be returned.</param>
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>The result of the SQL command.</returns>
    internal async Task<IDictionary<string, object?>?> ExecuteColumnKeyedDictionaryResultAsync(IContext context,
        IEnumerable<string> columns, CancellationToken cancel)
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new ColumnKeyedDictionaryResultProcessor(columns);
        var result = await RunCommandAsync<IDictionary<string, object?>>(context, processor, cancel)
            .ConfigureAwait(false);
        return result;
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
    internal IEnumerable<T>? ExecuteListResult<T>(IContext context)
    {
        if (context.CommandText.Length == 0) return default;
            
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
    internal T? ExecuteResult<T>(IContext context, T? defaultResult, IEnumerable<PropertyInfo>? collections)
    {
        if (context.CommandText.Length == 0) return default;
            
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
    internal T? ExecuteMapResult<T>(IContext context, IEnumerable<Tuple<PropertyInfo, Type>> map)
        where T : new()
    {
        if (context.CommandText.Length == 0) return default;
            
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
        if (context.CommandText.Length == 0) return default;
            
        return RunCommand<int>(context);
    }

    /// <summary>
    /// Executes the specified SQL command text and runs a custom delegate
    /// to process the results.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="context">A SQL command context which includes a SELECT command.</param>
    /// <param name="valueProvider">Custom delegate to create the result.</param>
    /// <returns>The result of the SQL command.</returns>
    internal T? ExecuteCustom<T>(IContext context, CreateEntity<T> valueProvider)
    {
        if (context.CommandText.Length == 0) return default;
            
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
    internal IDictionary<TKey, TValue>? ExecuteRowKeyedDictionaryResult<TKey, TValue>(IContext context)
        where TKey : notnull
    {
        if (context.CommandText.Length == 0) return default;
            
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
    internal IDictionary<string, object?>? ExecuteColumnKeyedDictionaryResult(IContext context,
        IEnumerable<string> columns)
    {
        if (context.CommandText.Length == 0) return default;
            
        var processor = new ColumnKeyedDictionaryResultProcessor(columns);
        return RunCommand<IDictionary<string, object?>>(context, processor);
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
        if (context.CommandText.Length == 0) return Enumerable.Empty<T>();
            
        return new ResultStream<T>(context, this);
    }

    /// <summary>
    /// Runs the specified SQL.
    /// </summary>
    /// <param name="context">The SQL context to run.</param>
    /// <param name="processors">The processors to use for the result sets.</param>
    /// <returns>A task for the operation.</returns>
    internal void RunCommandSet(IContext context, IList<ProcessorContext> processors)
    {
        if (context.CommandText.Length == 0) return;
            
        using var connection = AsyncHelper.RunSync(CreateConnectionAsync);
        using var command = connection.CreateCommand();
        // initialize command
        command.Connection = connection;
        command.CommandText = context.CommandText;
        AddParametersToCommand(command, context);

        using var scope = CreateScope(context);
        try
        {
            connection.Open();
            if (processors.Any() != true)
            {
                command.ExecuteNonQuery();
            }
            else
            {
                using var reader = command.ExecuteReader();
                var results = 0;
                foreach (var item in processors)
                {
                    if (results > 0 && !reader.NextResult())
                    {
                        throw new InvalidOperationException(
                            $"Expected {processors.Count} but result only contained {results}");
                    }

                    if (item.Deferred is null || item.Processer is null)
                        throw new InvalidOperationException("Null value in processor context");

                    item.Deferred.SetValue(item.Processer.Process(reader));
                    results++;
                }
            }

            connection.Close();
        }
        catch
        {
            scope?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }

    /// <summary>
    /// Runs the specified SQL as an asynchronous operation.
    /// </summary>
    /// <param name="context">The SQL context to run.</param>
    /// <param name="processors">The processors to use for the result sets.</param>
    /// <returns>A task for the operation.</returns>
    internal async Task RunCommandSetAsync(IContext context, IList<ProcessorContext> processors)
    {
        if (context.CommandText.Length == 0) return;
            
        await using var connection = await CreateConnectionAsync().ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        // initialize command
        command.Connection = connection;
        command.CommandText = context.CommandText;
        AddParametersToCommand(command, context);

        using var activity = CreateScope(context);
        try
        {
            await connection.OpenAsync().ConfigureAwait(false);
            if (processors.Any() != true)
            {
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            else
            {
                await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                var results = 0;
                foreach (var item in processors)
                {
                    if (results > 0 && !await reader.NextResultAsync().ConfigureAwait(false))
                    {
                        throw new InvalidOperationException(
                            $"Expected {processors.Count} but result only contained {results}");
                    }

                    if (item.Deferred is null || item.Processer is null)
                        throw new InvalidOperationException("Null value in processor context");

                    item.Deferred.SetValue(item.Processer.Process(reader));
                    results++;
                }
            }

            await connection.CloseAsync().ConfigureAwait(false);
        }
        catch
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
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
    /// <param name="cancel">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous task for the query result.</returns>
    private async Task<T?> RunCommandAsync<T>(IContext context, IResultProcessor? processor = null,
        CancellationToken cancel = default)
    {
        AssertCorrectReturnType<T>(processor);
            
        if (context.CommandText.Length == 0) return default;

        await using var connection = await CreateConnectionAsync().ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        // initialize command
        command.Connection = connection;
        command.CommandText = context.CommandText;
        AddParametersToCommand(command, context);

        object? result;

        using var activity = CreateScope(context);
        try
        {
            await connection.OpenAsync(cancel).ConfigureAwait(false);
            if (processor == null)
            {
                result = await command.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
            }
            else
            {
                await using var reader = await command.ExecuteReaderAsync(cancel).ConfigureAwait(false);
                result = processor.Process(reader);
            }

            await connection.CloseAsync().ConfigureAwait(false);
        }
        catch
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        return (T?) result;
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
    private T? RunCommand<T>(IContext context, IResultProcessor? processor = null)
    {
        AssertCorrectReturnType<T>(processor);
            
        if (context.CommandText.Length == 0) return default;

        using var connection = AsyncHelper.RunSync(CreateConnectionAsync);
        using var command = connection.CreateCommand();
        // initialize command
        command.Connection = connection;
        command.CommandText = context.CommandText;
        AddParametersToCommand(command, context);

        object? result;

        using var activity = CreateScope(context);
        try
        {
            connection.Open();
            if (processor == null)
            {
                result = command.ExecuteNonQuery();
            }
            else
            {
                using var reader = command.ExecuteReader();
                result = processor.Process(reader);
            }

            connection.Close();
        }
        catch
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        return (T?) result;
    }
        
    private Activity? CreateScope(IContext context)
    {
        var builder = _activitySource?.StartActivity(context.OperationName ?? "sql-command", ActivityKind.Client)
            ?.SetTag("db-type", "sql")
            .SetTag("component", "trancesql");

        return builder?.Start();
    }

    private static void AssertCorrectReturnType<T>(IResultProcessor? processor)
    {
        if (processor == null && typeof(T) != typeof(int))
        {
            throw new ArgumentException(
                $"Attempted to run a non-query command with the return type '{typeof(T).FullName}'. Non-query commands must return the type 'int'.",
                nameof(T));
        }
    }
}