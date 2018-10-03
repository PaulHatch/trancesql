using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    public interface IConstraint : ISqlElement { }

    public class UniqueConstraint : IConstraint
    {
        public string Name { get; set; }

        public ColumnCollection _on;
        public ColumnCollection On
        {
            get => _on = _on ?? new ColumnCollection();
            set => _on = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                context.Write("CONSTRAINT ");
                context.WriteIdentifier(Name);
                context.Write(' ');
            }
            context.Write("UNIQUE");
            if (_on?.Any() == true)
            {
                context.Write(" (");
                context.RenderDelimited(_on);
                context.Write(')');
            }
        }

        public override string ToString() => this.RenderDebug();
    }

    public class PrimaryKeyConstraint : IConstraint
    {
        public string Name { get; set; }

        public ColumnOrderCollection _on;
        public ColumnOrderCollection On
        {
            get => _on = _on ?? new ColumnOrderCollection();
            set => _on = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (_on?.Any() != true)
            {
                throw new InvalidCommandException("No columns specified for primary key constraint.");
            }

            if (!String.IsNullOrEmpty(Name))
            {
                context.Write("CONSTRAINT ");
                context.WriteIdentifier(Name);
                context.Write(' ');
            }
            context.Write("PRIMARY KEY (");
            context.RenderDelimited(_on);
            context.Write(')');
        }

        public override string ToString() => this.RenderDebug();
    }


    public enum DeleteBehavior
    {
        NoAction,
        Cascade,
        SetNull,
        SetDefault
    }

    public class ForeignKeyConstraint : IConstraint
    {
        public string Name { get; set; }

        public ColumnCollection _on;
        public ColumnCollection On
        {
            get => _on = _on ?? new ColumnCollection();
            set => _on = value;
        }

        public Table References { get; set; }

        public ColumnCollection _foriegnColumns;
        public ColumnCollection ForiegnColumns
        {
            get => _foriegnColumns = _foriegnColumns ?? new ColumnCollection();
            set => _foriegnColumns = value;
        }

        public DeleteBehavior OnDelete { get; set; } = DeleteBehavior.NoAction;

        void ISqlElement.Render(RenderContext context)
        {
            if (_on?.Any() != true)
            {
                throw new InvalidCommandException("No columns specified for primary key constraint.");
            }

            if (!String.IsNullOrEmpty(Name))
            {
                context.Write("CONSTRAINT ");
                context.WriteIdentifier(Name);
                context.Write(' ');
            }
            context.Write("FOREIGN KEY (");
            context.RenderDelimited(_on);
            context.Write(") REFERENCES ");
            context.Render(References);
            context.RenderDelimited(_on);
            context.Write(" (");
            context.RenderDelimited(_on);
            context.Write(')');

            switch (OnDelete)
            {
                case DeleteBehavior.Cascade:
                    context.Write(" ON DELETE CASCADE");
                    break;
                case DeleteBehavior.SetNull:
                    context.Write(" ON DELETE SET NULL");
                    break;
                case DeleteBehavior.SetDefault:
                    context.Write(" ON DELETE SET DEFAULT");
                    break;
                default:
                    break;
            }
        }

        public override string ToString() => this.RenderDebug();
    }

    public class CheckConstraint : IConstraint
    {
        public string Name { get; set; }

        private ConditionCollection _check;
        public ConditionCollection Check
        {
            get => _check = _check ?? new ConditionCollection();
            set => _check = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (_check?.Any() != true)
            {
                throw new InvalidCommandException("No condition specified for check constraint.");
            }

            if (!String.IsNullOrEmpty(Name))
            {
                context.Write("CONSTRAINT ");
                context.WriteIdentifier(Name);
                context.Write(' ');
            }
            context.Write("CHECK (");
            context.Render(_check);
            context.Write(')');
        }

        public override string ToString() => this.RenderDebug();
    }

    public class DefaultConstraint : IConstraint
    {
        public string Name { get; set; }

        public ISqlElement Value { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            if (Value == null)
            {
                throw new InvalidCommandException("No value specified for default constraint.");
            }

            if (!String.IsNullOrEmpty(Name))
            {
                context.Write("CONSTRAINT ");
                context.WriteIdentifier(Name);
                context.Write(' ');
            }
            context.Write("DEFAULT (");
            context.Render(Value);
            context.Write(')');
        }

        public override string ToString() => this.RenderDebug();
    }
}
