using static TranceSql.UsingStatic;
using Xunit;

namespace TranceSql.Test
{
    public class UpdateRenderingTest
    {
        [Fact]
        public void BasicUpdateRender()
        {
            var sut = new Update
            {
                Table = "Table",
                Set = {
                    { "Column1", 123 }
                }
            };

            var result = sut.ToString();

            Assert.Equal("UPDATE Table\nSET Column1 = @P1;", result);
        }

        [Fact]
        public void UpdateWithAliasRender()
        {
            var sut = new Update
            {
                Table = Table("Table").As("T"),
                Set = {
                    { "Column1", 123 }
                }
            };

            var result = sut.ToString();

            Assert.Equal("UPDATE Table T\nSET Column1 = @P1;", result);
        }

        [Fact]
        public void UpdateMultipleAssignmentsRender()
        {
            var sut = new Update
            {
                Table = Table("Table").As("T"),
                Set = {
                    { "Column1", 123 },
                    { "Column2", 123 }
                }
            };

            var result = sut.ToString();

            Assert.Equal("UPDATE Table T\nSET Column1 = @P1, Column2 = @P2;", result);
        }
    }
}
