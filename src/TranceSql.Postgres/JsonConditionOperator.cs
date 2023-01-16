namespace TranceSql.Postgres;

/// <summary>
/// Represents a type of Postgres operation on json or jsonb values.
/// </summary>
public enum JsonConditionOperator
{
    /// <summary>Represents a '@&gt;' operation.</summary>
    LeftContainsRight,
    /// <summary>Represents a '&lt;@' operation.</summary>
    RightContainsLeft,
    /// <summary>Represents a '?' operation.</summary>
    Contains,
    /// <summary>Represents a '?|' operation.</summary>
    ContainsAny
}