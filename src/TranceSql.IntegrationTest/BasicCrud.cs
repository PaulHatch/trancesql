using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranceSql.Sqlite;
using Xunit;
using static TranceSql.UsingStatic;

namespace TranceSql.IntegrationTest
{
    public class BasicCrud : IDisposable
    {
        private class Sample
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
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() },
                        { "column1", SqlType.From<string>() }
                    }
                }
            }.Execute();
        }

        private readonly Database _database;

        [Fact]
        public async Task UniqueConstraint()
        {
            // Create a table with a unique constraint and insert duplicates in to blow it up
            var sut = new Command(_database)
            {
                new CreateTable("unique_table")
                {
                    Columns =
                    {
                        { "column", SqlType.From<int>(), new UniqueConstraint() }
                    }
                },
                new Insert { Into = "unique_table", Columns = "column1", Values = { 1 } },
                new Insert { Into = "unique_table", Columns = "column1", Values = { 1 } }
            };

            var exception = await Assert.ThrowsAnyAsync<DbException>(() => sut.ExecuteAsync());
            Assert.Contains("unique", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task CheckConstraintInline()
        {
            // Create a table with a check constraint and insert one safe value
            await new Command(_database)
            {
                new CreateTable("check_table")
                {
                    Columns =
                    {
                        { "column", SqlType.From<int>(), new CheckConstraint(new Column("column") < new Constant(15)) }
                    }
                },
                new Insert { Into = "check_table", Columns = "column", Values = { 10 } }
            }.ExecuteAsync();

            // now add a value which violates the check constraint
            var sut = new Command(_database)
            {
                new Insert { Into = "check_table", Columns = "column", Values = { 20 } }
            };

            var exception = await Assert.ThrowsAnyAsync<DbException>(() => sut.ExecuteAsync());
            Assert.Contains("check", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task CheckConstraint()
        {
            // Create a table with a check constraint and insert one safe value
            await new Command(_database)
            {
                new CreateTable("check_table")
                {
                    Columns =
                    {
                        { "column", SqlType.From<int>() }
                    },
                    Constraints =
                    {
                        new CheckConstraint(new Column("column") < new Constant(15))
                    }
                },
                new Insert { Into = "check_table", Columns = "column", Values = { 10 } }
            }.ExecuteAsync();

            // now add a value which violates the check constraint
            var sut = new Command(_database)
            {
                new Insert { Into = "check_table", Columns = "column", Values = { 20 } }
            };

            var exception = await Assert.ThrowsAnyAsync<DbException>(() => sut.ExecuteAsync());
            Assert.Contains("check", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task NamedCheckConstraint()
        {
            // Create a table with a check constraint and insert one safe value
            await new Command(_database)
            {
                new CreateTable("check_table")
                {
                    Columns =
                    {
                        { "column", SqlType.From<int>() }
                    },
                    Constraints =
                    {
                        new CheckConstraint(new Column("column") < new Constant(15))
                    }
                },
                new Insert { Into = "check_table", Columns = "column", Values = { 10 } }
            }.ExecuteAsync();

            // now add a value which violates the check constraint
            var sut = new Command(_database)
            {
                new Insert { Into = "check_table", Columns = "column", Values = { 20 } }
            };

            var exception = await Assert.ThrowsAnyAsync<DbException>(() => sut.ExecuteAsync());
            Assert.Contains("check", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task ForeignKeyConstraint()
        {
            // Create tables with a foreign constraint
            await new Command(_database)
            {
                new CreateTable("first_table")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() },
                        { "second_table_id", SqlType.From<int>() }
                    },
                    Constraints = { new ForeignKeyConstraint("second_table_id", "second_table", "id") { OnDelete = DeleteBehavior.Cascade } }
                },
                new CreateTable("second_table")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() }
                    }
                },
                new Insert { Into = "second_table", Columns = "id", Values = { 55 } },
                new Insert { Into = "first_table", Columns = { "id", "second_table_id" }, Values = { 5, 55 } }
            }.ExecuteAsync();

            var firstCount = await new Command(_database)
            {
                new Select { Columns = { new Function("COUNT", new Column("*")) }, From = "first_table" }
            }.FetchAsync<int>();

            var deletedCount = await new Command(_database)
            {
                new Delete { From = "second_table" }
            }.ExecuteAsync();

            var secondCount = await new Command(_database)
            {
                new Select { Columns = { new Function("COUNT", new Column("*")) }, From = "first_table" }
            }.FetchAsync<int>();

            Assert.Equal(1, firstCount);
            Assert.Equal(1, deletedCount);
            Assert.Equal(0, secondCount);

            var sut = new Command(_database)
            {
                new Insert { Into = "first_table", Columns = { "id", "second_table_id" }, Values = { 5, 55 } }
            };

            var exception = await Assert.ThrowsAnyAsync<DbException>(() => sut.ExecuteAsync());
            Assert.Contains("foreign", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task DefaultValue()
        {
            // Ensure the default value is populated in a new row
            var sut = new Command(_database)
            {
                new CreateTable("default_table")
                {
                    Columns =
                    {
                        { "column1", SqlType.From<int>() },
                        { "column2", SqlType.From<string>(), new DefaultConstraint("hello world") }
                    }
                },
                new Insert { Into = "default_table", Columns = "column1", Values = { 1 } },
                new Select { Columns = "column2", From = "default_table", Where = new Column("column1") == new Value(1) }
            };

            var result = await sut.FetchAsync<string>();

            Assert.Equal("hello world", result);
        }


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

        [Fact]
        public void StreamingExecution()
        {
            var count = new Command(_database)
            {
                new Delete { From = "sample"},
                new Insert
                {
                    Into = "sample",
                    Columns = {"id", "column1" },
                    Values =
                    {
                        { 11, "data" },
                        { 12, "data" },
                        { 13, "data" },
                        { 14, "data" },
                        { 15, "data" },
                        { 16, "data" },
                        { 17, "data" },
                        { 18, "data" }
                    }
                },
                new Select { Columns = Count(), From = "sample" }
            }.Fetch<int>();

            var sut = new Command(_database)
            {
                new Select { Columns = { "id", "column1" }, From = "sample" }
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

        public void Dispose()
        {
            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }
        }
    }
}
