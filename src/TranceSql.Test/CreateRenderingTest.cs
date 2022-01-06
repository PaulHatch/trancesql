using Xunit;

namespace TranceSql.Test
{
    public class CreateRenderingTest
    {
        [Fact]
        public void BasicCreateTableRender()
        {
            var sut = new CreateTable("Table")
            {
                Columns =
                {
                    { "Column1", SqlType.From<string>(allowNull: false) }
                }
            };

            var result = sut.ToString();

            Assert.Equal("CREATE TABLE Table\n(\nColumn1 STRING NOT NULL\n);", result);
        }
    }
}
