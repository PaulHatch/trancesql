using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Default value extractor implementation provides resolution of 
    /// raw values into valid values for parameters.
    /// </summary>
    public class DefaultValueExtractor : IParameterValueExtractor
    {
        /// <summary>
        /// Sets the parameter value to be used for the given object.
        /// </summary>
        /// <param name="parameter">The parameter to be set.</param>
        /// <param name="value">The input value.</param>
        /// <returns>A value suitable to be used for a parameter</returns>
        public void SetValue(DbParameter parameter, object value)
        {
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                var valueType = value.GetType();
                
                parameter.Value = value;
            }
        }
    }
}
