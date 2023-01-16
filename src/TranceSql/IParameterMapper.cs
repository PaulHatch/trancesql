using System.Data.Common;

namespace TranceSql;

/// <summary>
/// Provides parameter value from object instances.
/// </summary>
public interface IParameterMapper
{
    /// <summary>
    /// Sets the parameter value to be used for the given object.
    /// </summary>
    /// <param name="parameter">The parameter to set.</param>
    /// <param name="value">The input value.</param>
    void SetValue(DbParameter parameter, object? value);
}