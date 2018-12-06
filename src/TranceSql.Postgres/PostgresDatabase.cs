using Npgsql;
using OpenTracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TranceSql.Processing;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Creates command parameters for a PostgreSQL database reference.
    /// </summary>
    public class PostgresDatabase : Database
    {
        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        public PostgresDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl)
            : this(asyncConnectionStringFactory, ttl, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public PostgresDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor)
            : this(asyncConnectionStringFactory, ttl, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, ITracer tracer)
            : this(asyncConnectionStringFactory, ttl, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="asyncConnectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public PostgresDatabase(Func<Task<string>> asyncConnectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(asyncConnectionStringFactory, ttl, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new PostgresDialect())
        {
        }


        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        public PostgresDatabase(Func<string> connectionStringFactory, TimeSpan ttl)
            : this(connectionStringFactory, ttl, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public PostgresDatabase(Func<string> connectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor)
            : this(connectionStringFactory, ttl, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(Func<string> connectionStringFactory, TimeSpan ttl, ITracer tracer)
            : this(connectionStringFactory, ttl, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="connectionStringFactory">A delegate which will be called
        /// to provide a connection string, this method will be called each time
        /// a new connection string is needed, allowing for rolling credentials.</param>
        /// <param name="ttl">The time to wait before refreshing the connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public PostgresDatabase(Func<string> connectionStringFactory, TimeSpan ttl, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionStringFactory, ttl, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo), new PostgresDialect())
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgresDatabase(string connectionString)
            : this(connectionString, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor)
            : this(connectionString, extractor, null)
        {
        }

        /// <summary>
        /// Creates command parameters for a Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(string connectionString, ITracer tracer)
            : this(connectionString, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for a Postgres database reference.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="extractor">The parameter value extractor.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(string connectionString, IParameterValueExtractor extractor, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, extractor ?? new DefaultValueExtractor(), tracer, ExtractDbInfo(connectionString)), new PostgresDialect())
        {
        }

        private static DbInfo ExtractDbInfo(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            return new DbInfo(builder.Host, builder.Database, builder.Username);
        }

        private static DbConnection GetConnection() => new NpgsqlConnection();


        private class ListenObservable : IObservable<string>
        {
            private readonly List<IObserver<string>> _observers = new List<IObserver<string>>();
            private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
            private bool _complete;

            public IDisposable Subscribe(IObserver<string> observer)
            {
                _lock.EnterWriteLock();
                try
                {
                    _observers.Add(observer);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
                return new ListenUnsubscriber(this, observer);
            }

            private class ListenUnsubscriber : IDisposable
            {
                private readonly ListenObservable _observable;
                private readonly IObserver<string> _observer;

                public ListenUnsubscriber(ListenObservable observable, IObserver<string> observer)
                {
                    _observable = observable;
                    _observer = observer;
                }

                public void Dispose()
                {
                    _observable.Unsubscribe(_observer);
                }
            }

            private void Unsubscribe(IObserver<string> observer)
            {
                _lock.EnterWriteLock();
                try
                {
                    _observers.Remove(observer);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            internal void OnCompleted()
            {
                _lock.EnterUpgradeableReadLock();
                try
                {
                    if (!_complete)
                    {
                        foreach (var observer in _observers)
                        {
                            observer.OnCompleted();
                        }
                        _lock.EnterWriteLock();
                        try
                        {
                            _complete = true;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            internal void OnError(Exception error)
            {
                _lock.EnterUpgradeableReadLock();
                try
                {
                    if (!_complete)
                    {
                        foreach (var observer in _observers)
                        {
                            observer.OnError(error);
                        }
                        _lock.EnterWriteLock();
                        try
                        {
                            _complete = true;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            internal void OnNext(string value)
            {
                _lock.EnterReadLock();
                try
                {
                    if (!_complete)
                    {
                        foreach (var observer in _observers)
                        {
                            observer.OnNext(value);
                        }
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Event listener for Postgres <code>listen</code> events. This
        /// listener should be disposed when complete to ensure the underlying
        /// connection is closed.
        /// </summary>
        public class PostgresEventListener : IDisposable
        {
            private NpgsqlConnection _connection;
            private readonly object _observableLocker = new object();
            private ListenObservable _observable = new ListenObservable();

            /// <summary>
            /// Initializes a new instance of the <see cref="PostgresEventListener"/> class.
            /// </summary>
            /// <param name="connection">The connection.</param>
            public PostgresEventListener(NpgsqlConnection connection)
            {
                _connection = connection;
                _connection.Notification += OnNotification;
                _connection.Disposed += OnConnectionDisposed;
                _connection.StateChange += OnStateChange;
            }

            private void OnStateChange(object sender, StateChangeEventArgs e)
            {
                if (e.CurrentState == ConnectionState.Broken)
                {
                    _observable.OnError(new PostgresException { MessageText = "The connection to the database was broken." });
                }
            }

            private void OnConnectionDisposed(object sender, EventArgs e)
            {
                _observable.OnCompleted();
            }

            private void OnNotification(object sender, NpgsqlNotificationEventArgs e)
            {
                _observable.OnNext(e.AdditionalInformation);
            }

            /// <summary>Gets an observable for this event listener.</summary>
            public IObservable<string> Observable => _observable;

            /// <summary>
            /// Returns a task which can be awaited to hold the listening connection.
            /// </summary>
            /// <param name="cancel">A cancellation token.</param>
            /// <returns></returns>
            public Task AwaitAsync(CancellationToken cancel = default)
            {
                return _connection.WaitAsync(cancel);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing,
            /// or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _connection?.Dispose();
                _connection = null;
            }
        }

        /// <summary>
        /// Creates a listener on the specified channel. When using a listener,
        /// it may be helpful to also turn on keepalive in the database connection
        /// string.
        /// </summary>
        /// <param name="channel">The channel to listen on.</param>
        /// <returns>An event listener for the specified channel.</returns>
        public async Task<PostgresEventListener> CreateListenerAsync(string channel)
        {
            var connection = await CreateConnectionAsync() as NpgsqlConnection;
            await connection.OpenAsync();

            var listener = new PostgresEventListener(connection);

            using (var command = new NpgsqlCommand($"LISTEN {Dialect.FormatIdentifier(channel)}", connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            return listener;
        }
    }
}
