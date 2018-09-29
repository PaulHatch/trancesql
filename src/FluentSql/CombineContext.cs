using System.Collections.Generic;
using System.Text;
using TranceSql.Language;

namespace TranceSql
{
    internal class CombineContext : IContext
    {
        StringBuilder _commandText = new StringBuilder();
        Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public string CommandText => _commandText.ToString();

        public IReadOnlyDictionary<string, object> ParameterValues => _parameters;

        public void Append(IContext context)
        {
            _commandText.Append(context.CommandText);
            foreach (var parameter in _parameters)
            {
                _parameters.Add(parameter.Key, parameter.Value);
            }
            
        }
    }
}