using System.Collections.Generic;

namespace TranceSql.Language
{
    public class JoinCollection : List<Join>
    {
        public void Add(JoinType joinType, string table, ICondition on)
        {
            Add(new Join(joinType, new Table(table), on));
        }

        public static implicit operator JoinCollection(Join join)
            => new JoinCollection { join };
    }
}