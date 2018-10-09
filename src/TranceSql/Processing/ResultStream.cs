using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using TranceSql.Language;

namespace TranceSql.Processing
{
    /// <summary>
    /// Streaming results enumeration.
    /// </summary>
    /// <typeparam name="T">Type of the enumeration.</typeparam>
    internal class ResultStream<T> : IEnumerable<T>
    {
        private IContext _context;
        private SqlCommandManager _manager;

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
        private bool _isSimpleType;
        private CreateEntity<T> _readEntity;
        private DbConnection _connection;
        private DbCommand _command;
        private DbDataReader _reader;
        private IDictionary<string, int> _map;

        public ResultStreamEnumerator(IContext context, SqlCommandManager manager)
        {
            _isSimpleType = EntityMapping.IsSimpleType<T>();
            _readEntity = _isSimpleType ? GetSimple : EntityMapping.GetEntityFunc<T>();


            _connection = manager.ConnectionFactory();
            _connection.ConnectionString = manager.ConnectionString;
            _command = _connection.CreateCommand();
            _command.Connection = _connection;
            _command.CommandText = context.CommandText;
            manager.AddParametersToCommand(_command, context);
            _connection.Open();
            _reader = _command.ExecuteReader();
            _map = _isSimpleType ? null : EntityMapping.MapDbReaderColumns(_reader);
        }
        
        private T GetSimple(DbDataReader reader, IDictionary<string, int> map)
        {
            var result = reader[0];
            if (result is T match)
            {
                return match;
            }
            else if (Convert.IsDBNull(result))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current { get; private set; }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _command?.Dispose();
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
            if (_reader.Read())
            {
                Current = _readEntity(_reader, _map);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in
        /// the collection.
        /// </summary>
        public void Reset()
        {
            Current = default(T);
            _reader?.Close();
            _command?.Cancel();
            _reader = _command.ExecuteReader();
            _map = _isSimpleType ? null : EntityMapping.MapDbReaderColumns(_reader);
        }
    }
}
