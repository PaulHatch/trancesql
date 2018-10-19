using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TranceSql.Processing
{
    /// <summary>
    /// Defines a custom mapper for entity properties. This allows consumers to provide 
    /// customized entity mapping of common types or based on custom attributes.
    /// </summary>
    public interface ICustomEntityMap
    {
        /// <summary>
        /// Return true if this entity property mapper should be used for the specified property.
        /// </summary>
        /// <param name="property">The property to test.</param>
        /// <returns>True if this map should be used to resolve the result.</returns>
        bool DoesApply(PropertyInfo property);

        /// <summary>
        /// Gets the expression that maps the expect value of the property from the database reader.
        /// </summary>
        /// <param name="property">The property being assigned.</param>
        /// <param name="genericReadHelperGet">The MethodInfo for the generic method ReadHelper.Get&lt;&gt;().</param>
        /// <param name="readerParam">The parameter containing the database DbDataReader.</param>
        /// <param name="mapParam">The map parameter.</param>
        /// <returns>An expression mapping the row of the given DBDataReader to a result.</returns>
        Expression GetExpression(PropertyInfo property, MethodInfo genericReadHelperGet, ParameterExpression readerParam, ParameterExpression mapParam);
    }
}
