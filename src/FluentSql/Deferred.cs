using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql
{
    /// <summary>
    /// Base class for  deferred 
    /// </summary>
    public abstract class Deferred
    {
        internal abstract void SetValue(object value);
    }

    public sealed class Deferred<T> : Deferred
    {
        private T _result;
        private DeferContext _context;

        internal Deferred(DeferContext context)
        {
            _context = context;
        }

        public T GetResult()
        {
            _context.Run();

            return _result;
        }

        public async Task<T> GetResultAsync()
        {
            await _context.RunAsync();
            
            return _result;
        }

        internal override void SetValue(object value)
        {
            _result = (T)value;
        }
    }
}
