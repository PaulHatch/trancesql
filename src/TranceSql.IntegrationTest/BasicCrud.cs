using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranceSql.Sqlite;
using Xunit;
using Xunit.Abstractions;
using static TranceSql.UsingStatic;

namespace TranceSql.IntegrationTest
{
    public class BasicCrud : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public BasicCrud(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(new TestTracer(helper));
        }

        [Fact]
        public void UniqueConstraint()
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
                new Insert { Into = "unique_table", Columns = "column", Values = { 1 } },
                new Insert { Into = "unique_table", Columns = "column", Values = { 1 } }
            };

            var exception = Assert.ThrowsAny<DbException>(() => sut.Execute());
            Assert.Contains("unique", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void CheckConstraintInline()
        {
            // Create a table with a check constraint and insert one safe value
            new Command(_database)
            {
                new CreateTable("check_table_inline")
                {
                    Columns =
                    {
                        { "column", SqlType.From<int>(), new CheckConstraint(new Column("column") < new Constant(15)) }
                    }
                },
                new Insert { Into = "check_table_inline", Columns = "column", Values = { 10 } }
            }.Execute();

            // now add a value which violates the check constraint
            var sut = new Command(_database)
            {
                new Insert { Into = "check_table_inline", Columns = "column", Values = { 20 } }
            };

            var exception = Assert.ThrowsAny<DbException>(() => sut.Execute());
            Assert.Contains("check", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void CheckConstraint()
        {
            // Create a table with a check constraint and insert one safe value
            new Command(_database)
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
            }.Execute();

            // now add a value which violates the check constraint
            var sut = new Command(_database)
            {
                new Insert { Into = "check_table", Columns = "column", Values = { 20 } }
            };

            var exception = Assert.ThrowsAny<DbException>(() => sut.Execute());
            Assert.Contains("check", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void NamedCheckConstraint()
        {
            // Create a table with a check constraint and insert one safe value
            new Command(_database)
            {
                new CreateTable("named_check_table")
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
                new Insert { Into = "named_check_table", Columns = "column", Values = { 10 } }
            }.Execute();

            // now add a value which violates the check constraint
            var sut = new Command(_database)
            {
                new Insert { Into = "named_check_table", Columns = "column", Values = { 20 } }
            };

            var exception = Assert.ThrowsAny<DbException>(() => sut.Execute());
            Assert.Contains("check", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void ForeignKeyConstraint()
        {
            // Create tables with a foreign constraint
            new Command(_database)
            {
                new CreateTable("second_table")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() }
                    }
                },
                new CreateTable("first_table")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() },
                        { "second_table_id", SqlType.From<int>() }
                    },
                    Constraints = { new ForeignKeyConstraint("second_table_id", "second_table", "id") { OnDelete = DeleteBehavior.Cascade } }
                },
                new Insert { Into = "second_table", Columns = "id", Values = { 55 } },
                new Insert { Into = "first_table", Columns = { "id", "second_table_id" }, Values = { 5, 55 } }
            }.Execute();

            var firstCount = new Command(_database)
            {
                new Select { Columns = { new Function("COUNT", new Column("*")) }, From = "first_table" }
            }.Fetch<int>();

            var deletedCount = new Command(_database)
            {
                new Delete { From = "second_table" }
            }.Execute();

            var secondCount = new Command(_database)
            {
                new Select { Columns = { new Function("COUNT", new Column("*")) }, From = "first_table" }
            }.Fetch<int>();

            Assert.Equal(1, firstCount);
            Assert.Equal(1, deletedCount);
            Assert.Equal(0, secondCount);

            var sut = new Command(_database)
            {
                new Insert { Into = "first_table", Columns = { "id", "second_table_id" }, Values = { 5, 55 } }
            };

            var exception = Assert.ThrowsAny<DbException>(() => sut.Execute());
            Assert.Contains("foreign", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void DefaultValue()
        {
            // Ensure the default value is populated in a new row
            var sut = new Command(_database)
            {
                new CreateTable("default_table")
                {
                    Columns =
                    {
                        { "column1", SqlType.From<int>() },
                        { "column2", SqlType.From<string>(false), new DefaultConstraint("hello world") }
                    }
                },
                new Insert { Into = "default_table", Columns = "column1", Values = { 1 } },
                new Select { Columns = "column2", From = "default_table", Where = new Column("column1") == new Value(1) }
            };

            var result = sut.Fetch<string>();

            Assert.Equal("hello world", result);
        }


        [Fact]
        public void BasicTest()
        {
            var count = new Command(_database)
            {
                new Insert
                {
                    Into = "sample",
                    Columns = { "id", "column1" },
                    Values = { 1, "hello world" }
                }
            }.Execute();

            Assert.Equal(1, count);

            var sample = new Command(_database)
            {
                new Select
                {
                    From = "sample",
                    Columns = new Column("column1"),
                    Where = Condition.Equal("id", 1)
                }
            }.Fetch<Sample>();

            Assert.Equal("hello world", sample.Column1);

            count = new Command(_database)
            {
                new Update
                {
                    Table = "sample",
                    Set = { { "column1", "hallo welt" } },
                    Where = Condition.Equal("id", 1)
                }
            }.Execute();

            Assert.Equal(1, count);

            sample = new Command(_database)
            {
                new Select
                {
                    From = "sample",
                    Columns = new Column("column1"),
                    Where = Condition.Equal("id", 1)
                }
            }.Fetch<Sample>();

            Assert.Equal("hallo welt", sample.Column1);

            count = new Command(_database)
            {
                new Delete
                {
                    From = "sample",
                    Where = Condition.Equal("id", 1)
                }
            }.Execute();

            Assert.Equal(1, count);

            sample = new Command(_database)
            {
                new Select
                {
                    From = "sample",
                    Columns = new Column("column1"),
                    Where = Condition.Equal("id", 1)
                }
            }.Fetch<Sample>();

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

        [Fact]
        public void OutputUpdate()
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
                    Columns = { { "column1", SqlType.From<int>() } }
                },
                new Insert { Into = "update_output_table", Columns = "column1", Values = { 1 } },
                new Update { Table = "update_output_table", Set = { { "column1", 2 } }, Where = new Column("column1") == new Value(1), Returning = "column1" }
            };

            var result = sut.Fetch<int>();

            Assert.Equal(2, result);
        }

        [Fact]
        public void OutputInsert()
        {
            if(_database.Dialect.OutputType == OutputType.None)
            {
                // don't test dialects which do not support this type
                return;
            }

            var sut = new Command(_database)
            {
                new CreateTable("insert_output_table")
                {
                    Columns = { { "column1", SqlType.From<int>() } }
                },
                new Insert { Into = "insert_output_table", Columns = "column1", Values = { 2 }, Returning = "column1" },
            };

            var result = sut.Fetch<int>();

            Assert.Equal(2, result);
        }
    }
}
