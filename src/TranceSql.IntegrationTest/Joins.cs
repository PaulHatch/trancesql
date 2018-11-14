using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    public class Joins : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public Joins(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(new TestTracer(helper));
        }

        [Fact]
        public async Task BasicInnerJoin()
        {
            var sut = new Command(_database)
            {
                new CreateTable("inner_table_1")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() },
                        { "column", SqlType.From<string>() }
                    }
                },
                new CreateTable("inner_table_2")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() },
                        { "fk_id", SqlType.From<int>() },
                        { "column", SqlType.From<string>() }
                    },
                    Constraints =
                    {
                        new ForeignKeyConstraint("fk_id", "inner_table_1", "id")
                    }
                },
                new Insert{ Into = "inner_table_1", Columns = {"id", "column" }, Values = { { 1, "test" } } },
                new Insert{ Into = "inner_table_2", Columns = {"id", "fk_id", "column" }, Values = { { 1, 1, "result" } } },
                new Select
                {
                    Columns = { {"t1","column" }, { "t2", "column" } },
                    From = new Table("inner_table_1").As("t1"),
                    Join = new Join(JoinType.Join, new Table("inner_table_2").As("t2"), new Column("t1", "id") == new Column("t2", "fk_id"))
                }
            };

            var result = await sut.FetchRowKeyedDictionaryAsync<string, string>();

            Assert.Equal("result", result["test"]);
        }
    }
}
