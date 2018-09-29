using System;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql.Language
{
    public class Join : ISqlElement
    {
        private JoinType JoinType { get; }
        private Table Table { get; set; }
        private ConditionCollection On = new ConditionCollection();

        public Join(JoinType joinType)
        {
            JoinType = joinType;
        }

        public Join(JoinType joinType, Table table, ICondition on)
        {
            JoinType = joinType;
            Table = table;
            On.Add(on);
        }

        void ISqlElement.Render(RenderContext context)
        {
            switch (JoinType)
            {
                case JoinType.Join:
                    context.Write("\nJOIN ");
                    break;
                case JoinType.Inner:
                    context.Write("\nINNER JOIN ");
                    break;
                case JoinType.LeftOuter:
                    context.Write("\nLEFT JOIN ");
                    break;
                case JoinType.RightOuter:
                    context.Write("\nRIGHT JOIN ");
                    break;
                case JoinType.FullOuter:
                    context.Write("\nOUT JOIN ");
                    break;
                case JoinType.Cross:
                    context.Write("\nCROSS JOIN ");
                    break;
                default:
                    break;
            }

            context.Render(Table);

            if (On.Any())
            {
                context.Write(" ON ");
                On.RenderCollection(context);
            }
        }

        public override string ToString() => this.RenderDebug();
    }
}