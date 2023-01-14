using System.Threading.Tasks;

namespace TranceSql
{
    /// <summary>
    /// Defines a deferred result of SQL execution.
    /// </summary>
    internal interface IDeferred
    {
        /// <summary>
        /// Sets the value of this deferred result.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void SetValue(object? value);
    }

    /// <summary>
    /// Represents the deferred result of SQL execution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Deferred<T> : IDeferred
    {
        private T? _result;
        private DeferContext _context;

        internal Deferred(DeferContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the result of this operation using synchronous execution if this 
        /// is the first result from the deferrer context being retrieved.
        /// </summary>
        public T? Result
        {
            get
            {
                _context.Run();
                return _result;
            }
        }

        /// <summary>
        /// Gets the result of this operation using asynchronous execution if this
        /// is the first result from the deferrer context being retrieved.
        /// </summary>
        public Task<T?> ResultAsync => GetResultAsync();

        private async Task<T?> GetResultAsync()
        {
            await _context.RunAsync().ConfigureAwait(false);
            return _result;
        }

        void IDeferred.SetValue(object? value)
        {
            _result = (T?)value;
        }
    }
}
