using System;
using System.Linq;

namespace TranceSql;

/// <summary>
/// Represents a table constraint definition.
/// </summary>
public interface IConstraint : ISqlElement
{
}

/// <summary>
/// Represents a unique constraint definition.
/// </summary>
public class UniqueConstraint : IConstraint
{
    /// <summary>
    /// Gets or sets the name of this constraint.
    /// </summary>
    public string Name { get; set; }

    private ColumnCollection _on;

    /// <summary>
    /// Gets or sets the columns to create the constraint on.
    /// </summary>
    public ColumnCollection On
    {
        get => _on ??= new ColumnCollection();
        set => _on = value;
    }

    void ISqlElement.Render(RenderContext context)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            context.Write("CONSTRAINT ");
            context.WriteIdentifier(Name);
            context.Write(' ');
        }

        context.Write("UNIQUE");
        if (_on?.Any() == true)
        {
            context.Write(" (");
            context.RenderDelimited(_on, columnNamesOnly: true);
            context.Write(')');
        }
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => Extensions.CreateDebugString(this);
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

    private ColumnOrderCollection _on;

    /// <summary>
    /// Gets or sets the columns to create this primary key on.
    /// </summary>
    public ColumnOrderCollection On
    {
        get => _on ??= new ColumnOrderCollection();
        set => _on = value;
    }

    void ISqlElement.Render(RenderContext context)
    {
        if (!string.IsNullOrEmpty(Name))
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
    public override string ToString() => Extensions.CreateDebugString(this);
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
    public ForeignKeyConstraint()
    {
    }

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
    public ForeignKeyConstraint(string column, string referenceSchema, string referenceTable,
        string referenceColumn)
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
        get => _columns ??= new ColumnCollection();
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
        get => _referencesColumns ??= new ColumnCollection();
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

        if (!string.IsNullOrEmpty(Name))
        {
            context.Write("CONSTRAINT ");
            context.WriteIdentifier(Name);
            context.Write(' ');
        }

        context.Write("FOREIGN KEY (");
        context.RenderDelimited(_columns, columnNamesOnly: true);
        context.Write(") REFERENCES ");
        context.Render(References);
        context.Write(" (");
        context.RenderDelimited(_referencesColumns, columnNamesOnly: true);
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
    public override string ToString() => Extensions.CreateDebugString(this);
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
    public CheckConstraint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckConstraint"/> class.
    /// </summary>
    /// <param name="condition">
    /// The condition for the check, this must not include any <see cref="Parameter"/>
    /// or <see cref="Value"/> instances.
    /// </param>
    public CheckConstraint(Condition condition)
    {
        Check = condition;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckConstraint"/> class.
    /// </summary>
    /// <param name="condition">
    /// The condition for the check, this must not include any <see cref="Parameter"/>
    /// or <see cref="Value"/> instances.
    /// </param>
    public CheckConstraint(ConditionPair condition)
    {
        Check = condition;
    }

    /// <summary>
    /// Gets or sets the check condition for this constraint, this must not include 
    /// any <see cref="Parameter"/> or <see cref="Value"/> instances.
    /// </summary>
    public FilterClause Check { get; set; }

    void ISqlElement.Render(RenderContext context)
    {
        if (Check == null)
        {
            throw new InvalidCommandException("No condition specified for check constraint.");
        }

        if (!string.IsNullOrEmpty(Name))
        {
            context.Write("CONSTRAINT ");
            context.WriteIdentifier(Name);
            context.Write(' ');
        }

        context.Write("CHECK (");
        context.Render(Check.Value);
        context.Write(')');
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => Extensions.CreateDebugString(this);
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
        if (((object) Value) == null)
        {
            throw new InvalidCommandException("No value specified for default constraint.");
        }

        if (!string.IsNullOrEmpty(Name))
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
    public override string ToString() => Extensions.CreateDebugString(this);
}