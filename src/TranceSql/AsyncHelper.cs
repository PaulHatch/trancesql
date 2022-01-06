using System;
using System.Threading;
using System.Threading.Tasks;

namespace TranceSql
{
    /// <summary>
    /// Helper class to run async methods within a sync process.
    /// </summary>
    internal static class AsyncHelper
    {
        private static readonly TaskFactory _taskFactory = new(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        /// <summary>
        /// Runs an async Task method synchronously.
        /// </summary>
        /// <param name="task">Task method to run.</param>
        public static void RunSync(Func<Task> task)
            => _taskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        /// Runs an async Task method synchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="task">Task method to run.</param>
        /// <returns>Result of the provided task.</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task)
            => _taskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
}
