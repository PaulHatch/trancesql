using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TranceSql;

/// <summary>
/// Table schema which provides a snake_case column name transformation.
/// </summary>
public class TransformingTableSchema : TableSchema
{
    private readonly TransformType _type;

    /// <inheritdoc/>
    public TransformingTableSchema(TransformType type, string schema, string name) : base(schema, name)
    {
        _type = type;
    }

    /// <inheritdoc/>
    protected override string ColumnNameTransformer(string? name)
    {
        if (name is null)
            return "";

        switch (_type)
        {
            case TransformType.SnakeCase:
                var words = name.Split(new[] {"-", " "}, StringSplitOptions.RemoveEmptyEntries);
                var firstWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                    m => m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
                var remainingWords = words.Skip(1)
                    .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                    .ToArray();
                return $"{firstWord}_{string.Join("_", remainingWords)}";
        }

        return name;

    }
}

/// <summary>
/// Column name transformation type.
/// </summary>
public enum TransformType
{
    /// <summary>Transform to snake_case.</summary>
    SnakeCase,
    /// <summary>Transform to camelCase.</summary>
    CamelCase,
    /// <summary>Transform to PascalCase.</summary>
    PascalCase
}