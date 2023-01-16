using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static TranceSql.UsingStatic;

namespace TranceSql.IntegrationTest;

public class BasicCrudAsync : IClassFixture<DatabaseFixture>
{
    protected readonly Database _database;

    public BasicCrudAsync(DatabaseFixture db, ITestOutputHelper helper)
    {
        _database = db.GetDatabase(helper);
    }


    [Fact]
    public async Task BasicTest()
    {
        var count = await new Command(_database)
        {
            new Insert
            {
                Into = "sample",
                Columns = {"id", "column1"},
                Values = {1, "hello world"}
            }
        }.ExecuteAsync();

        Assert.Equal(1, count);

        var sample = await new Command(_database)
        {
            new Select
            {
                From = "sample",
                Columns = new Column("column1"),
                Where = Condition.Equal("id", 1)
            }
        }.FetchAsync<Sample>();

        Assert.Equal("hello world", sample.Column1);

        count = await new Command(_database)
        {
            new Update
            {
                Table = "sample",
                Set = {{"column1", "hallo welt"}},
                Where = Condition.Equal("id", 1)
            }
        }.ExecuteAsync();

        Assert.Equal(1, count);

        sample = await new Command(_database)
        {
            new Select
            {
                From = "sample",
                Columns = new Column("column1"),
                Where = Condition.Equal("id", 1)
            }
        }.FetchAsync<Sample>();

        Assert.Equal("hallo welt", sample.Column1);

        count = await new Command(_database)
        {
            new Delete
            {
                From = "sample",
                Where = Condition.Equal("id", 1)
            }
        }.ExecuteAsync();

        Assert.Equal(1, count);

        sample = await new Command(_database)
        {
            new Select
            {
                From = "sample",
                Columns = new Column("column1"),
                Where = Condition.Equal("id", 1)
            }
        }.FetchAsync<Sample>();

        Assert.Null(sample);
    }

    [Fact]
    public void StreamingExecution()
    {
        var count = new Command(_database)
        {
            new Delete {From = "sample"},
            new Insert
            {
                Into = "sample",
                Columns = {"id", "column1"},
                Values =
                {
                    {11, "data"},
                    {12, "data"},
                    {13, "data"},
                    {14, "data"},
                    {15, "data"},
                    {16, "data"},
                    {17, "data"},
                    {18, "data"}
                }
            },
            new Select {Columns = Count(), From = "sample"}
        }.Fetch<int>();

        var sut = new Command(_database)
        {
            new Select {Columns = {"id", "column1"}, From = "sample"}
        }.FetchStream<Sample>();

        var count1 = sut.Count();
        var count2 = 0;
        foreach (var item in sut)
        {
            count2++;
        }

        Assert.Equal(count, count1);
        Assert.Equal(count, count2);
    }

    [Fact]
    public async Task OutputUpdate()
    {
        if (_database.Dialect.OutputType == OutputType.None)
        {
            // don't test dialects which do not support this type
            return;
        }

        var sut = new Command(_database)
        {
            new CreateTable("update_output_table")
            {
                Columns = {{"column1", SqlType.From<int>()}}
            },
            new Insert {Into = "update_output_table", Columns = "column1", Values = {1}},
            new Update
            {
                Table = "update_output_table", Set = {{"column1", 2}},
                Where = new Column("column1") == new Value(1), Returning = "column1"
            }
        };

        var result = await sut.FetchAsync<int>();

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task ShippedUpdate()
    {
        var sut = new Command(_database)
        {
            new Update
            {
                Table = "invalid_table",
                Set = {{"column1", 2, false}}
            }
        };

        var result = await sut.ExecuteAsync();
    }

    [Fact]
    public async Task OutputInsert()
    {
        if (_database.Dialect.OutputType == OutputType.None)
        {
            // don't test dialects which do not support this type
            return;
        }

        var sut = new Command(_database)
        {
            new CreateTable("insert_output_table")
            {
                Columns = {{"column1", SqlType.From<int>()}}
            },
            new Insert {Into = "insert_output_table", Columns = "column1", Values = {2}, Returning = "column1"},
        };

        var result = await sut.FetchAsync<int>();

        Assert.Equal(2, result);
    }
}