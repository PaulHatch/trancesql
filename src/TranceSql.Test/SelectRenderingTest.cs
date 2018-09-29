using System;
using TranceSql.Language;
using Xunit;
using static TranceSql.Language.UsingStatic;

namespace TranceSql.Test
{
    public class SelectRenderingTest
    {
        [Fact]
        public void BasicSelectRender()
        {
            var sut = new Select
            {
                Columns = { "Column1", "Column2" },
                From = { "Table" }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT Column1, Column2\nFROM Table;", result);
        }

        [Fact]
        public void LimitSelectRender()
        {
            var sut = new Select
            {
                Columns = { "Column1" },
                From = { "Table" },
                Limit = 10
            };

            var result = sut.ToString();

            Assert.Equal("SELECT Column1\nFROM Table\nLIMIT 10;", result);
        }

        [Fact]
        public void TableColumnSelectRender()
        {
            var sut = new Select
            {
                Columns = { { "Table", "Column1" } },
                From = { "Table" }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT Table.Column1\nFROM Table;", result);
        }

        [Fact]
        public void SchemaTableColumnSelectRender()
        {
            var sut = new Select
            {
                Columns = { { "dbo", "Table", "Column1" } },
                From = { "Table" }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT dbo.Table.Column1\nFROM Table;", result);
        }

        [Fact]
        public void GroupBySelectRender()
        {
            var sut = new Select
            {
                Columns = { "Column1" },
                From = { "Table" },
                GroupBy = { "Column1" }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT Column1\nFROM Table\nGROUP BY Column1;", result);
        }

        [Fact]
        public void HavingSelectRender()
        {
            var sut = new Select
            {
                Columns = { "Column1" },
                From = { "Table" },
                GroupBy = { "Column1" },
                Having = Condition.Equal("Column1", 123)
            };

            var result = sut.ToString();

            Assert.Equal("SELECT Column1\nFROM Table\nGROUP BY Column1\nHAVING Column1 = @P1;", result);
        }

        [Fact]
        public void WhereColumnEqualsValueRender()
        {
            var sut = new Select
            {
                Columns = { "*" },
                From = { "Table" },
                Where = Condition.Equal("Column1", 123)
            };

            var result = sut.ToString();

            Assert.Equal("SELECT *\nFROM Table\nWHERE Column1 = @P1;", result);
        }

        [Fact]
        public void WhereColumnEqualsValueAndRender()
        {
            var sut = new Select
            {
                Columns = "*",
                From = "Table",
                Where = {
                    Condition.Equal("Column1", 123),
                    Column("Column2") == Value(123)
                }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT *\nFROM Table\nWHERE Column1 = @P1 AND Column2 = @P2;", result);
        }

        [Fact]
        public void WhereColumnEqualsValueOrRender()
        {
            var sut = new Select
            {
                Columns = { "*" },
                From = { "Table" },
                Where = {
                    Condition.Equal("Column1", 123),
                    Or.Equal("Column2", 123)
                }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT *\nFROM Table\nWHERE Column1 = @P1 OR Column2 = @P2;", result);
        }

        [Fact]
        public void SelectConstantRender()
        {
            var sut = new Select
            {
                Columns = {
                    Constant.Unsafe("some value"),
                    new Constant(1),
                    new Constant(1.5),
                    new Constant(false).As("Value"),
                }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT 'some value', 1, 1.5, CAST(0 AS BIT) AS Value;", result);
        }


        [Fact]
        public void InlineParameterAssignmentRender()
        {
            var parameter = Parameter("P");
            var sut = new Select
            {
                Columns = { new Assignment(parameter, Column("Column1")) },
                From = { "Table" }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT @P = Column1\nFROM Table;", result);
        }

        [Fact]
        public void OrderByRender()
        {
            var sut = new Select
            {
                Columns = { "*" },
                From = { "Table" },
                OrderBy = { "Column2" }
            };

            var result = sut.ToString();

            Assert.Equal("SELECT *\nFROM Table\nORDER BY Column2;", result);
        }

        [Fact]
        public void DynamicWhereRender()
        {
            var sut = new Select
            {
                Columns = { "*" },
                From = { "Table" },
                Where = Column("Column1") + new Raw("1") > new Raw("2") &
                        Column("Column2") < Parameter("P")
            };

            var result = sut.ToString();

            Assert.Equal("SELECT *\nFROM Table\nWHERE Column1 + 1 > 2 AND Column2 < @P;", result);
        }

        [Fact]
        public void TableAliasRender()
        {
            var sut = new Select
            {
                Columns = { "*" },
                From = Table("Table").As("T")
            };

            var result = sut.ToString();

            Assert.Equal("SELECT *\nFROM Table AS T;", result);
        }

        [Fact]
        public void ColumnAliasRender()
        {
            var sut = new Select
            {
                Columns = { Column("Column1").As("C") },
                From = Table("Table")
            };

            var result = sut.ToString();

            Assert.Equal("SELECT Column1 AS C\nFROM Table;", result);
        }

        [Theory]
        [InlineData(LimitBehavior.Top, "SELECT TOP 5 Column\nFROM Table;")]
        [InlineData(LimitBehavior.Limit, "SELECT Column\nFROM Table\nLIMIT 5;")]
        [InlineData(LimitBehavior.LimitAndOffset, "SELECT Column\nFROM Table\nLIMIT 5;")]
        [InlineData(LimitBehavior.FetchFirst, "SELECT Column\nFROM Table\nFETCH FIRST 5 ROWS ONLY;")]
        [InlineData(LimitBehavior.FetchFirstAndOffset, "SELECT Column\nFROM Table\nFETCH FIRST 5 ROWS ONLY;")]
        [InlineData(LimitBehavior.RowNum, "SELECT Column\nFROM (\nSELECT Column,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS rownumber\nFROM Table)\nWHERE rownumber <= 5;")]
        [InlineData(LimitBehavior.RowNumAutomatic, "SELECT *\nFROM (\nSELECT Column\nFROM Table)\nWHERE RowNum <= 5;")]
        public void LimitBehaviorTypes(LimitBehavior behavior, string expectedResult)
        {
            var dialect = new GenericDialect { LimitBehavior = behavior };
            var context = new RenderContext(dialect);

            var sut = new Select
            {
                Columns = "Column",
                From = "Table",
                Limit = 5
            } as ISqlElement;

            sut.Render(context);

            Assert.Equal(expectedResult, context.CommandText);
        }
    }
}
