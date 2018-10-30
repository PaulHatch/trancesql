using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql
{
    /// <summary>
    /// Represents the execution context for deferred SQL operations.
    /// </summary>
    public sealed class DeferContext
    {
        private CombineContext _context = new CombineContext();
        private List<ProcessorContext> _processors = new List<ProcessorContext>();

        /// <summary>
        /// Gets the database for execution of commands within this context.
        /// </summary>
        internal Database Database { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has executed.
        /// </summary>
        internal bool HasExecuted { get; private set; }

        /// <summary>Gets or sets the current index value for dynamic parameters.</summary>
        internal int ParameterIndex { get; set; }

        /// <summary>
        /// Represents the execution context for deferred SQL operations.
        /// </summary>
        /// <param name="database">The database to execute commands requested in this context.</param>
        internal DeferContext(Database database)
        {
            Database = database;
            ParameterIndex = 1;
        }

        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="context">The SQL context.</param>
        /// <param name="processor">The processor.</param>
        /// <returns>Deferred&lt;T&gt;.</returns>
        private Deferred<T> DeferCommand<T>(IContext context, IResultProcessor processor)
        {
            _context.Append(context);
            var deferredResult = new Deferred<T>(this);
            _processors.Add(new ProcessorContext { Processer = processor, Deferred = deferredResult });
            return deferredResult;
        }


        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="context">The SQL context.</param>
        /// <returns>Deferred&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        internal Deferred<IEnumerable<TResult>> ExecuteListResultDeferred<TResult>(IContext context)
        {
            return DeferCommand<IEnumerable<TResult>>(context, new ListResultProcessor<TResult>());
        }

        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="context">The SQL context.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>Deferred&lt;TResult&gt;.</returns>
        internal Deferred<TResult> ExecuteResultDeferred<TResult>(IContext context, object defaultValue, IEnumerable<PropertyInfo> properties)
        {
            return DeferCommand<TResult>(context, new SingleResultProcessor<TResult>((TResult)defaultValue, properties));
        }

        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="context">The SQL context.</param>
        /// <param name="mappedProperties">The mapped properties.</param>
        /// <returns>Deferred&lt;TResult&gt;.</returns>
        internal Deferred<TResult> ExecuteMapResultDeferred<TResult>(IContext context, IEnumerable<Tuple<PropertyInfo, Type>> mappedProperties)
            where TResult : new()
        {
            return DeferCommand<TResult>(context, new MappedResultProcessor<TResult>(mappedProperties));
        }

        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="context">The SQL context.</param>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>Deferred&lt;TResult&gt;.</returns>
        internal Deferred<TResult> ExecuteCustomDeferred<TResult>(IContext context, CreateEntity<TResult> valueProvider)
        {
            return DeferCommand<TResult>(context, new CustomResultProcessor<TResult>(valueProvider));
        }

        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="context">The SQL context.</param>
        /// <returns>Deferred&lt;IDictionary&lt;TKey, TValue&gt;&gt;.</returns>
        internal Deferred<IDictionary<TKey, TValue>> ExecuteRowKeyedDictionaryResultDeferred<TKey, TValue>(IContext context)
        {
            return DeferCommand<IDictionary<TKey, TValue>>(context, new RowKeyedDictionaryResultProcessor<TKey, TValue>());
        }

        /// <summary>
        /// Queues a command and returns a deferred reference to the result.
        /// </summary>
        /// <param name="context">The SQL context.</param>
        /// <param name="columns">The columns.</param>
        /// <returns>Deferred&lt;IDictionary&lt;System.String, System.Object&gt;&gt;.</returns>
        internal Deferred<IDictionary<string, object>> ExecuteColumnKeyedDictionaryResultDeferred(IContext context, string[] columns)
        {
            return DeferCommand<IDictionary<string, object>>(context, new ColumnKeyedDictionaryResultProcessor(columns));
        }

        /// <summary>
        /// Queues a command without a result.
        /// </summary>
        /// <param name="context">The SQL context.</param>
        internal void ExecuteDeferred(IContext context)
        {
            _context.Append(context);
        }

        /// <summary>
        /// Executes the SQL for the current context and processes the results as an asynchronous operation.
        /// </summary>
        /// <returns>A task for the operation.</returns>
        internal async Task RunAsync()
        {
            await Database.Manager.RunCommandSetAsync(_context, _processors);
            HasExecuted = true;
        }

        /// <summary>
        /// Executes the SQL for the current context and processes the results.
        /// </summary>
        internal void Run()
        {
            Database.Manager.RunCommandSet(_context, _processors);
            HasExecuted = true;
        }
    }
}
