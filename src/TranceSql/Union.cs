using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql;

/// <summary>
/// Represents the type of a <see cref="Union"/>.
/// </summary>
public enum UnionType
{
    /// <summary>
    /// A UNION type union.
    /// </summary>
    Union,
    /// <summary>
    /// A UNION ALL type union.
    /// </summary>
    UnionAll,
    /// <summary>
    /// A MINUS type union.
    /// </summary>
    Minus,
    /// <summary>
    /// An INTERSECT type union.
    /// </summary>
    Intersect
}

/// <summary>
/// Represents the union of two or more queries.
/// </summary>
public class Union : IEnumerable<ISqlElement>, ISqlStatement
{
    private List<(ISqlElement element, UnionType type)> _statements = new();
    private UnionType? _nextType = null;

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<ISqlElement> GetEnumerator() => _statements.Select(s => s.element).GetEnumerator();
        
    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => _statements.Select(s => s.element).GetEnumerator();


    /// <summary>
    /// Adds the specified element to the union statement.
    /// </summary>
    /// <param name="element">The element to add.</param>
    public void Add(ISqlElement element)
    {
        _statements.Add((element, _nextType ?? UnionType.Union));
        _nextType = null;
    }

    /// <summary>
    /// Adds the specified element to the union statement.
    /// </summary>
    /// <param name="type">The type of union.</param>
    /// <param name="element">The element to add.</param>
    public void Add(UnionType type, ISqlElement element)
    {
        _statements.Add((element, type));
        _nextType = null;
    }

    /// <summary>
    /// Configures the type of join created for the next element
    /// added to this statement.
    /// </summary>
    /// <param name="type">The type of the union.</param>
    public void Add(UnionType type) => _nextType = type;

    void ISqlElement.Render(RenderContext context)
    {
        if (context.Mode == RenderMode.Nested)
        {
            context.Write('(');
        }
        using (context.EnterChildMode(RenderMode.MultiStatment))
        {
            var first = true;
            foreach (var (element, type) in _statements)
            {
                if (!first)
                {
                    context.WriteLine();
                    switch (type)
                    {
                        case UnionType.Union:
                            context.WriteLine("UNION");
                            break;
                        case UnionType.UnionAll:
                            context.WriteLine("UNION ALL");
                            break;
                        case UnionType.Minus:
                            context.WriteLine("MINUS");
                            break;
                        case UnionType.Intersect:
                            context.WriteLine("INTERSECT");
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown union type '{type}'");
                    }
                }

                context.Render(element);

                first = false;
            }
        }
        switch (context.Mode)
        {
            case RenderMode.Statment:
                context.Write(';');
                break;
            case RenderMode.Nested:
                context.Write(')');
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