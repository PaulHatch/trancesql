using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace TranceSql.Processing
{
    /// <summary>
    /// Streaming results enumeration.
    /// </summary>
    /// <typeparam name="T">Type of the enumeration.</typeparam>
    internal class ResultStream<T> : IEnumerable<T>
    {
        private readonly IContext _context;
        private readonly SqlCommandManager _manager;

        public ResultStream(IContext context, SqlCommandManager manager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ResultStreamEnumerator<T>(_context, _manager);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    /// <summary>
    /// Enumerator for a streaming results enumeration.
    /// </summary>
    /// <typeparam name="T">Type of the enumeration.</typeparam>
    internal class ResultStreamEnumerator<T> : IEnumerator<T>
    {
        private readonly bool _isSimpleType;
        private readonly CreateEntity<T> _readEntity;
        private readonly DbConnection _connection;
        private readonly DbCommand _command;
        private DbDataReader _reader;
        private IDictionary<string, int>? _map;

        public ResultStreamEnumerator(IContext context, SqlCommandManager manager)
        {
            Current = default!;
            _isSimpleType = EntityMapping.IsSimpleType<T>();
            _readEntity = _isSimpleType ? GetSimple : EntityMapping.GetEntityFunc<T>();

            _connection = AsyncHelper.RunSync(manager.CreateConnectionAsync);
            _command = _connection.CreateCommand();
            _command.Connection = _connection;
            _command.CommandText = context.CommandText;
            manager.AddParametersToCommand(_command, context);
            _connection.Open();
            _reader = _command.ExecuteReader();
            _map = _isSimpleType ? null : EntityMapping.MapDbReaderColumns(_reader);
        }

        private static T GetSimple(DbDataReader reader, IDictionary<string, int> map)
        {
            var result = reader[0];
            if (result is T match)
            {
                return match;
            }

            if (Convert.IsDBNull(result))
            {
                return default!;
            }

            return (T) Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current { get; private set; }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => Current ?? default!;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _command.Dispose();
            _connection.Dispose();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the
        /// enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (!_reader.Read())
            {
                return false;
            }

            Current = _readEntity(_reader, _map ?? new Dictionary<string, int>());
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in
        /// the collection.
        /// </summary>
        public void Reset()
        {
            Current = default!;
            _reader.Close();
            _command.Cancel();
            _reader = _command.ExecuteReader();
            _map = _isSimpleType ? null : EntityMapping.MapDbReaderColumns(_reader);
        }
    }
}