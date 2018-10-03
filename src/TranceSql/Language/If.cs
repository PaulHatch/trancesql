using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class If : ISqlStatement
    {
        public If()
        {

        }

        public If(ConditionCollection condition)
        {
            Conditions = condition;
        }

        public If(Condition condition)
        {
            Conditions = condition;
        }

        private ConditionCollection _conditions;
        public ConditionCollection Conditions
        {
            get => _conditions = _conditions ?? new ConditionCollection();
            set => _conditions = value;
        }

        /// <summary>
        /// Gets or sets the statement to execute, to specify multiple statements,
        /// use a <see cref="StatementBlock"/>.
        /// </summary>
        public ISqlStatement Then { get; set; }

        /// <summary>
        /// Gets or sets the statement to execute if conditions are not met, to specify
        /// multiple statements, use a <see cref="StatementBlock"/>.
        /// </summary>
        public ISqlStatement Else { get; set; }


        void ISqlElement.Render(RenderContext context)
        {
            context.Write("IF (");
            context.Render(Conditions);
            context.WriteLine(")");
            context.Render(Then);
            if (Else != null)
            {
                context.WriteLine();
                context.WriteLine("ELSE");
                context.Render(Else);
            }


        }

        public override string ToString() => this.RenderDebug();
    }
}
