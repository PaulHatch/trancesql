using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class CaseStatement : ExpressionElement, ISqlElement
    {
        public List<Case> Cases { get; } = new List<Case>();
        public ISqlElement Else { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            Console.WriteLine("(CASE");

            foreach (var item in Cases)
            {
                Console.WriteLine(" WHEN ");
                item.When.RenderCollection(context);
                Console.WriteLine(" THEN ");
                context.Render(item.Then);
            }

            if (Else != null)
            {
                Console.WriteLine(" ELSE ");
                context.Render(Else);
            }

            Console.WriteLine(" END)");
        }
    }

    public class Case
    {
        public ConditionCollection When { get; } = new ConditionCollection();
        public ISqlElement Then { get; set; }
    }
}
