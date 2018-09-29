namespace TranceSql.Language
{
    public interface ICondition : ISqlElement
    {
        BooleanOperator BooleanOperator { get; set;  }
    }
}
