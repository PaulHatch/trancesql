using System.Threading.Tasks;
using TranceSql.Postgres;
using Xunit;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    [Trait("dialect", "Postgres")]
    public class Postgres : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public Postgres(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(new TestTracer(helper));
        }

        [Fact]
        public async Task OnConflictCanExecute()
        {
            var sut = new Command(_database)
            {
                new CreateTable("unique_table")
                {
                    Columns =
                    {
                        { "column", SqlType.From<int>(), new UniqueConstraint() }
                    }
                },
                new Insert { Into = "unique_table", Columns = "column", Values = { 1 } },
                new Insert { Into = "unique_table", Columns = "column", Values = { 1 } }
                .OnConflict(
                    new [] {
                        new Column("column")
                    },
                    new Update {
                        Set = { { "column", 2 } }
                    }),
                new Select { Limit = 1, Columns = "column", From = "unique_table" }
            };

            var result = await sut.FetchAsync<int>();

            Assert.Equal(2, result);
        }

    }
}
