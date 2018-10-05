using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a table constraint definition.
    /// </summary>
    public interface IConstraint : ISqlElement { }

    /// <summary>
    /// Represents a unique constraint definition.
    /// </summary>
    public class UniqueConstraint : IConstraint
    {
        /// <summary>
        /// Gets or sets the name of this constraint.
        /// </summary>
        public string Name { get; set; }

        public ColumnCollection _on;
        /// <summary>
        /// Gets or sets the columns to create the constraint on.
        /// </summary>
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }

    /// <summary>
    /// Represents a primary key constraint.
    /// </summary>
    public class PrimaryKeyConstraint : IConstraint
    {
        /// <summary>
        /// Gets or sets the name of this constraint.
        /// </summary>
        public string Name { get; set; }

        public ColumnOrderCollection _on;
        /// <summary>
        /// Gets or sets the columns to create this primary key on.
        /// </summary>
        public ColumnOrderCollection On
        {
            get => _on = _on ?? new ColumnOrderCollection();
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

            context.Write("PRIMARY KEY");

            if (_on?.Any() == true)
            {
                context.Write('(');
                context.RenderDelimited(_on);
                context.Write(')');
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }

    /// <summary>
    /// On delete behavior for foreign key constraints.
    /// </summary>
    public enum DeleteBehavior
    {
        /// <summary>
        /// Specifies no action on delete.
        /// </summary>
        NoAction,
        /// <summary>
        /// Specifies cascading deletes.
        /// </summary>
        Cascade,
        /// <summary>
        /// Specifies that the foreign key column should be set to null on delete.
        /// </summary>
        SetNull,
        /// <summary>
        /// Specifies that the foreign key column should be set to the column's 
        /// default value on delete.
        /// </summary>
        SetDefault
    }

    /// <summary>
    /// Represents a foreign key constraint.
    /// </summary>
    public class ForeignKeyConstraint : IConstraint
    {
        /// <summary>
        /// Gets or sets the name of the constraint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyConstraint"/> class.
        /// </summary>
        public ForeignKeyConstraint() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyConstraint"/> class.
        /// </summary>
        /// <param name="column">The column to create a foreign key relationship on.</param>
        /// <param name="referenceTable">The table to reference.</param>
        /// <param name="referenceColumn">The column to reference.</param>
        public ForeignKeyConstraint(string column, string referenceTable, string referenceColumn)
        {
            Columns = column;
            References = referenceTable;
            ReferencesColumns = referenceColumn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyConstraint" /> class.
        /// </summary>
        /// <param name="column">The column to create a foreign key relationship on.</param>
        /// <param name="referenceSchema">The schema of the table being referenced.</param>
        /// <param name="referenceTable">The table to reference.</param>
        /// <param name="referenceColumn">The column to reference.</param>
        /// <exception cref="System.ArgumentNullException">referenceColumn</exception>
        public ForeignKeyConstraint(string column, string referenceSchema, string referenceTable, string referenceColumn)
        {
            Columns = column;
            References = new Table(referenceSchema, referenceTable);
            ReferencesColumns = referenceColumn;
        }

        private ColumnCollection _columns;
        /// <summary>
        /// Gets or sets the columns  to create a foreign key relationship on.
        /// </summary>
        public ColumnCollection Columns
        {
            get => _columns = _columns ?? new ColumnCollection();
            set => _columns = value;
        }

        /// <summary>
        /// Gets or sets the table to references.
        /// </summary>
        public Table References { get; set; }

        private ColumnCollection _referencesColumns;
        /// <summary>
        /// Gets or sets the columns to references.
        /// </summary>
        public ColumnCollection ReferencesColumns
        {
            get => _referencesColumns = _referencesColumns ?? new ColumnCollection();
            set => _referencesColumns = value;
        }

        /// <summary>
        /// Gets or sets the on delete behavior for this relationship.
        /// </summary>
        public DeleteBehavior OnDelete { get; set; } = DeleteBehavior.NoAction;

        void ISqlElement.Render(RenderContext context)
        {
            if (_columns?.Any() != true)
            {
                throw new InvalidCommandException("No columns specified for foreign key constraint.");
            }
            if (References == null)
            {
                throw new InvalidCommandException("No reference table specified for foreign key constraint.");
            }
            if (_referencesColumns?.Any() != true)
            {
                throw new InvalidCommandException("No reference columns specified for foreign key constraint.");
            }

            if (!String.IsNullOrEmpty(Name))
            {
                context.Write("CONSTRAINT ");
                context.WriteIdentifier(Name);
                context.Write(' ');
            }
            context.Write("FOREIGN KEY (");
            context.RenderDelimited(_columns);
            context.Write(") REFERENCES ");
            context.Render(References);
            context.Write(" (");
            context.RenderDelimited(_referencesColumns);
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }

    /// <summary>
    /// Represents a check constraint.
    /// </summary>
    public class CheckConstraint : IConstraint
    {
        /// <summary>
        /// Gets or sets the name of the constraint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckConstraint"/> class.
        /// </summary>
        public CheckConstraint() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckConstraint"/> class.
        /// </summary>
        /// <param name="conditions">
        /// The conditions for the check, this must not include any <see cref="Parameter"/>
        /// or <see cref="Value"/> instances.
        /// </param>
        public CheckConstraint(ConditionCollection conditions)
        {
            Check = conditions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckConstraint"/> class.
        /// </summary>
        /// <param name="conditions">
        /// The conditions for the check, this must not include any <see cref="Parameter"/>
        /// or <see cref="Value"/> instances.
        /// </param>
        public CheckConstraint(Condition conditions)
        {
            Check = conditions;
        }

        private ConditionCollection _check;
        /// <summary>
        /// Gets or sets the check condition for this constraint, this must not include 
        /// any <see cref="Parameter"/> or <see cref="Value"/> instances.
        /// </summary>
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }

    /// <summary>
    /// Represents a default constraint.
    /// </summary>
    public class DefaultConstraint : IConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConstraint"/> class.
        /// </summary>
        /// <param name="value">The default value.</param>
        public DefaultConstraint(object value)
        {
            Value = value is string text ? Constant.Unsafe(text) : new Constant(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConstraint"/> class.
        /// </summary>
        /// <param name="value">The default value.</param>
        public DefaultConstraint(Constant value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the name of the constraint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the constant value to assign.
        /// </summary>
        public Constant Value { get; set; }

        void ISqlElement.Render(RenderContext context)
        {
            if (((object)Value) == null)
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}
