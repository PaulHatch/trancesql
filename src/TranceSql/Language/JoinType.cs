using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public enum JoinType
    {
        Join,
        Inner,
        LeftOuter,
        RightOuter,
        FullOuter,
        Cross
    }
}
