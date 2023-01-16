using System.Reflection;

namespace TranceSql.Processing;

/// <summary>
/// Defines a custom mapper for entity properties. This allows consumers to provide 
/// customized entity mapping of common types or based on custom attributes. A custom
/// map will not be applied when mapping to ValueTuples.
/// </summary>
public interface ICustomBinder
{
    /// <summary>
    /// Return true if this entity property mapper should be used for the specified property.
    /// This method will be called only once per
    /// </summary>
    /// <param name="property">The property to test.</param>
    /// <returns>True if this map should be used to resolve the result.</returns>
    bool DoesApply(PropertyInfo property);

    /// <summary>
    /// Called to create a value to be assigned 
    /// </summary>
    /// <param name="property">The property being assigned.</param>
    /// <param name="value">Value from reader.</param>
    /// <returns>An expression mapping the row of the given DBDataReader to a result.</returns>
    object MapValue(PropertyInfo property, object value);
}