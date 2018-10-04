using System;
using System.IO;
using System.Threading.Tasks;
using TranceSql.Language;
using TranceSql.Sqlite;
using Xunit;

namespace TranceSql.IntegrationTest
{
    public class BasicCrud : IDisposable
    {
        class Sample
        {
            public int ID { get; set; }
            public string Column1 { get; set; }
        }

        public BasicCrud()
        {
            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }
            _database = new SqliteDatabase("Data Source=test.db");
            new Command(_database)
            {
                new CreateTable("sample")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>() },
                        { "column1", SqlType.From<string>() }
                    }
                }
            }.Execute();
        }

        Database _database;

        [Fact]
        public async Task BasicTestAsync()
        {
            var count = await new Command(_database)
            {
                new Insert
                {
                    Into = "sample",
                    Columns = { "id", "column1" },
                    Values = { 1, "hello world" }
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
                    Set = { { "column1", "hallo welt" } },
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

        public void Dispose()
        {
            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }
        }
    }
}
