﻿using Xunit;

namespace TranceSql.Test;

public class DropRenderingTest
{
    [Fact]
    public void BasicDropTableRender()
    {
        var sut = new Drop(DropType.Table, "Table");

        var result = sut.ToString();

        Assert.Equal("DROP TABLE Table;", result);
    }
}