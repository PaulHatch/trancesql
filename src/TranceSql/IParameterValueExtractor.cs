using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql
{
    /// <summary>
    /// Provides parameter value from object instances.
    /// </summary>
    public interface IParameterValueExtractor
    {
        /// <summary>
        /// Sets the parameter value to be used for the given object.
        /// </summary>
        /// <param name="parameter">The parameter to set.</param>
        /// <param name="value">The input value.</param>
        void SetValue(DbParameter parameter, object value);
    }
}
