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
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        public PostgresDatabase(PostgresRollingCredentials rollingCredentials)
            : this(rollingCredentials, null, null)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public PostgresDatabase(PostgresRollingCredentials rollingCredentials, IParameterMapper parameterMapper)
            : this(rollingCredentials, parameterMapper, null)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public PostgresDatabase(PostgresRollingCredentials rollingCredentials, ITracer tracer)
            : this(rollingCredentials, null, tracer)
        {
        }

        /// <summary>
        /// Creates command parameters for Postgres database reference.
        /// </summary>
        /// <param name="rollingCredentials">
        /// A connection string provider which uses rolling credentials such as
        /// dynamic credentials from a Vault database provider.
        /// </param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.</param>
        public PostgresDatabase(PostgresRollingCredentials rollingCredentials, IParameterMapper parameterMapper, ITracer tracer)
            : base(new SqlCommandManager(rollingCredentials, GetConnection, parameterMapper ?? new DefaultParameterMapper(), tracer, ExtractDbInfo), new PostgresDialect())
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
        /// <param name="parameterMapper">The parameter mapper.</param>
        public PostgresDatabase(string connectionString, IParameterMapper parameterMapper)
            : this(connectionString, parameterMapper, null)
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
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="tracer">
        /// The OpenTracing tracer instance to use. If this value is null the global tracer will
        /// be used instead.
        /// </param>
        public PostgresDatabase(string connectionString, IParameterMapper parameterMapper, ITracer tracer)
            : base(new SqlCommandManager(connectionString, GetConnection, parameterMapper ?? new DefaultParameterMapper(), tracer, ExtractDbInfo(connectionString)), new PostgresDialect())
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
                    _lock.ExitUpgradeableReadLock();
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
                    _lock.ExitUpgradeableReadLock();
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
                    _observable.OnError(new ListenerException("The connection to the database was broken."));
                }
            }

            private void OnConnectionDisposed(object sender, EventArgs e)
            {
                _observable.OnCompleted();
            }

            private void OnNotification(object sender, NpgsqlNotificationEventArgs e)
            {
                _observable.OnNext(e.Payload);
            }

            /// <summary>Gets an observable for this event listener.</summary>
            public IObservable<string> Observable => _observable;

            /// <summary>
            /// Returns a task which can be awaited to hold the listening connection.
            /// </summary>
            /// <param name="cancel">A cancellation token.</param>
            /// <returns></returns>
            public async Task AwaitAsync(CancellationToken cancel = default)
            {
                while (!cancel.IsCancellationRequested)
                {
                    await _connection.WaitAsync(cancel);
                }
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
