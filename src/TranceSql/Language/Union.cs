using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{
    /// <summary>
    /// 
    /// </summary>
    public enum UnionType
    {
        Union,
        UnionAll,
        Minus,
        Intersect
    }

    /// <summary>
    /// Represents the union of two or more queries.
    /// </summary>
    public class Union : IEnumerable<ISqlElement>, ISqlStatement
    {
        private List<(ISqlElement element, UnionType type)> _statements = new List<(ISqlElement element, UnionType type)>();
        private UnionType? _nextType = null;

        public IEnumerator<ISqlElement> GetEnumerator() => _statements.Select(s => s.element).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _statements.Select(s => s.element).GetEnumerator();



        public void Add(ISqlElement element)
        {
            _statements.Add((element, _nextType ?? UnionType.Union));
            _nextType = null;
        }

        public void Add(UnionType type, ISqlElement element)
        {
            _statements.Add((element, type));
            _nextType = null;
        }

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
                foreach (var statement in _statements)
                {
                    if (!first)
                    {
                        context.WriteLine();
                        switch (statement.type)
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
                                throw new InvalidOperationException($"Unknown union type '{statement.type}'");
                        }
                    }

                    context.Render(statement.element);

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

        public override string ToString() => this.RenderDebug();
    }
}
