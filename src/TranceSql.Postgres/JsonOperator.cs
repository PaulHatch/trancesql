namespace TranceSql.Postgres
{
    /// <summary>
    /// Represents a type of Postgres operation on json or jsonb values.
    /// </summary>
    public enum JsonOperator
    {
        /// <summary>Represents a '-&gt;' operation.</summary>
        Get,
        /// <summary>Represents a '-&gt;&gt;' operation.</summary>
        GetAsText,
        /// <summary>Represents a '#>' operation.</summary>
        GetByPath,
        /// <summary>Represents a '#&gt;&gt;' operation.</summary>
        GetByPathAsText,
        /// <summary>Represents a '@&gt;' operation.</summary>
        LeftContainsRight,
        /// <summary>Represents a '&lt;@' operation.</summary>
        RightContainsLeft,
        /// <summary>Represents a '?' operation.</summary>
        Contains,
        /// <summary>Represents a '?|' operation.</summary>
        ContainsAny,
        /// <summary>Represents a '||' operation.</summary>
        Concat,
        /// <summary>Represents a '-' operation.</summary>
        Delete,
        /// <summary>Represents a '#-' operation.</summary>
        DeletePath
    }
}