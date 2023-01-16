using TranceSql.Processing;
using Xunit;

namespace TranceSql.Test;

public class DefaultComparerTest
{
    [Fact]
    public void MatchesCorrectly()
    {
        Matches("theSame", "thesame");
        Matches("theSame", "thesame");
        Matches("the_same", "thesame");
        Matches("the_same", "thesame___");
        Matches("___thesame", "thesame");

        DoesNotMatch("the_first", "thesecond");
        DoesNotMatch("the_first", "theSecond");
    }

    private void Matches(string left, string right)
    {
        Assert.True(EntityMapping.ColumnPropertyComparer?.Equals(left, right));
        Assert.True(EntityMapping.ColumnPropertyComparer?.Equals(right, left));
    }

    private void DoesNotMatch(string left, string right)
    {
        Assert.False(EntityMapping.ColumnPropertyComparer?.Equals(left, right));
        Assert.False(EntityMapping.ColumnPropertyComparer?.Equals(right, left));
    }

}