using Xunit;

namespace TranceSql.Test
{
    public class DeleteRenderingTest
    {
        [Fact]
        public void BasicDeleteRender()
        {
            var sut = new Delete
            {
                From = "Table"
            };

            var result = sut.ToString();

            Assert.Equal("DELETE FROM Table;", result);
        }

        [Fact]
        public void WhereRender()
        {
            var sut = new Delete
            {
                From = "Table",
                Where = Condition.Equal("Column1", 123)
            };

            var result = sut.ToString();

            Assert.Equal("DELETE FROM Table\nWHERE Column1 = @P1;", result);
        }

    }
}
