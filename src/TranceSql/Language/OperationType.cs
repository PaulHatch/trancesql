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
        /// <summary>Equal</summary>
        Equal,
        /// <summary>Not Equal</summary>
        NotEqual,
        /// <summary>Greater Than</summary>
        GreaterThan,
        /// <summary>Greater Than Or Equal</summary>
        GreaterThanOrEqual,
        /// <summary>Less Than</summary>
        LessThan,
        /// <summary>Less Than Or Equal</summary>
        LessThanOrEqual,
        /// <summary>In</summary>
        In,
        /// <summary>Exists</summary>
        Exists,
        /// <summary>Not Exists</summary>
        NotExists,
        /// <summary>Not In</summary>
        NotIn,
        /// <summary>Is Null</summary>
        IsNull,
        /// <summary>Is Not Null</summary>
        IsNotNull
    }
}
