using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents the type of a <see cref="Join" />
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// A default join.
        /// </summary>
        Join,
        /// <summary>
        /// An inner join.
        /// </summary>
        Inner,
        /// <summary>
        /// A left outer join.
        /// </summary>
        LeftOuter,
        /// <summary>
        /// A right outer join.
        /// </summary>
        RightOuter,
        /// <summary>
        /// A full outer join.
        /// </summary>
        FullOuter,
        /// <summary>
        /// A cross join.
        /// </summary>
        Cross
    }
}
