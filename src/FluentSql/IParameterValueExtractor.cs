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
        /// <param name="value">The input value.</param>
        /// <returns>A value suitable to be used for a parameter</returns>
        void SetValue(DbParameter parameter, object value);
    }
}
