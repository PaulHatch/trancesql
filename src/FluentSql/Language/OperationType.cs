using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents the type of operation used between two sides of a filter clause.
    /// </summary>
    public enum OperationType
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        In,
        Exists,
        NotExists,
        NotIn,
        IsNull,
        IsNotNull
    }
}
