using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranceSql.Language
{

    public class Select : ISqlStatement, IDataSource
    {
        public long? Offset { get; set; }
        public long? Limit { get; set; }

        public bool Distinct { get; set; }

        public ColumnCollection _columns;
        public ColumnCollection Columns
        {
            get => _columns = _columns ?? new ColumnCollection();
            set => _columns = value;
        }

        private DataSourceCollection _from;
        public DataSourceCollection From
        {
            get => _from = _from ?? new DataSourceCollection();
            set => _from = value;
        }

        private JoinCollection _join;
        public JoinCollection Join
        {
            get => _join = _join ?? new JoinCollection();
            set => _join = value;
        }

        private ConditionCollection _where;
        public ConditionCollection Where {
            get => _where = _where ?? new ConditionCollection();
            set => _where = value;
        }

        public ColumnCollection _orderBy;
        public ColumnCollection OrderBy
        {
            get => _orderBy = _orderBy ?? new ColumnCollection();
            set => _orderBy = value;
        }


        public ColumnCollection _groupBy;
        public ColumnCollection GroupBy
        {
            get => _groupBy = _groupBy ?? new ColumnCollection();
            set => _groupBy = value;
        }

        private ConditionCollection _having;
        public ConditionCollection Having
        {
            get => _having = _having ?? new ConditionCollection();
            set => _having = value;
        }

        void ISqlElement.Render(RenderContext context)
        {
            if (context.Mode == RenderMode.Nested)
            {
                context.Write('(');
            }

            using (context.EnterChildMode(RenderMode.Nested))
            {

                // Write TOP statement
                if (Limit.HasValue)
                {
                    switch (context.Dialect.LimitBehavior)
                    {
                        case LimitBehavior.RowNum:
                            context.Write("SELECT ");
                            context.RenderDelimited(Columns);
                            context.WriteLine();
                            context.WriteLine("FROM (");
                            break;
                        case LimitBehavior.RowNumAutomatic:
                            context.WriteLine("SELECT *");
                            context.WriteLine("FROM (");
                            break;
                        default:
                            break;
                    }
                }

                context.Write("SELECT ");

                if (Distinct)
                {
                    context.Write("DISTINCT ");
                }

                // Write TOP statement
                if (Limit.HasValue)
                {
                    switch (context.Dialect.LimitBehavior)
                    {
                        case LimitBehavior.Top:
                            context.Write($"TOP {Limit} ");
                            break;
                        default:
                            break;
                    }
                }

                // Write the columns
                context.RenderDelimited(Columns);

                string rowNumberName = null;

                if (Limit.HasValue && context.Dialect.LimitBehavior == LimitBehavior.RowNum)
                {
                    rowNumberName = "rownumber";

                    // TODO: Perhaps check if this name already appears in the select list
                    // and change the name if so.

                    context.Write(",ROW_NUMBER() OVER (ORDER BY ");
                    if (_orderBy?.Any() == true)
                    {
                        context.RenderDelimited(OrderBy);
                    }
                    else
                    {
                        context.Write("(SELECT 1)");
                    }
                    context.Write($") AS {rowNumberName}");
                }

                if (_from?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("FROM ");
                    context.RenderDelimited(From);
                }
                if (_where?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("WHERE ");
                    Where.RenderCollection(context);
                }

                if (_orderBy?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("ORDER BY ");
                    context.RenderDelimited(OrderBy);
                }

                if (_groupBy?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("GROUP BY ");
                    context.RenderDelimited(GroupBy);
                }
                if (_having?.Any() == true)
                {
                    context.WriteLine();
                    context.Write("HAVING ");
                    Having.RenderCollection(context);
                }

                if (Offset.HasValue)
                {
                    switch (context.Dialect.OffsetBehavior)
                    {
                        case OffsetBehavior.Offset:
                            context.WriteLine();
                            context.Write($"OFFSET {Offset}");
                            break;
                        default:
                            throw new InvalidCommandException($"An offset was specified for a select command, but the current language dialect's limit behavior '{context.Dialect.LimitBehavior}' does not support offsets.");
                    }
                }

                if (Limit.HasValue)
                {
                    switch (context.Dialect.LimitBehavior)
                    {
                        case LimitBehavior.FetchFirst:
                            context.WriteLine();
                            context.Write($"FETCH FIRST {Limit} ROWS ONLY");
                            break;
                        case LimitBehavior.Limit:
                            context.WriteLine();
                            context.Write($"LIMIT {Limit}");
                            break;
                        case LimitBehavior.RowNum:
                        case LimitBehavior.RowNumAutomatic:
                            context.WriteLine(")");
                            context.Write($"WHERE {rowNumberName ?? "RowNum"} <= {Limit}");
                            break;
                        default:
                            break;
                    }
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
