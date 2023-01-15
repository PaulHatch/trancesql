using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    [Trait("dialect", "ANY")]
    public class DeferredExecution : IClassFixture<DatabaseFixture>
    {
        private readonly Database _database;

        public DeferredExecution(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(helper);
        }

        [Fact]
        public void BasicDeferredResults()
        {
            // setup

            new Command(_database)
            {
                CreateTable.From<Sample>("deferred_table"),
                new Insert {Into = "deferred_table", Columns = {"ID", "Column1"}, Values = {1, "Test"}},
            }.Execute();

            // test

            using var context = _database.CreateDeferContext();

            var command1 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_table"}
            };

            var command2 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_table"}
            };

            var results1 = command1.FetchListDeferred<Sample>();
            var results2 = command2.FetchListDeferred<Sample>();

            Assert.NotEmpty(results1.Result);
            Assert.NotEmpty(results2.Result);
        }

        [Fact]
        public void DeferredInlineResults()
        {
            // setup

            new Command(_database)
            {
                CreateTable.From<Sample>("deferred_inline_table"),
                new Insert {Into = "deferred_inline_table", Columns = {"ID", "Column1"}, Values = {1, "Test"}},
            }.Execute();

            // test

            using var context = _database.CreateDeferContext();

            var results1 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_inline_table"}
            }.FetchListDeferred<Sample>();

            var results2 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_inline_table"}
            }.FetchListDeferred<Sample>();

            Assert.NotEmpty(results1.Result);
            Assert.NotEmpty(results2.Result);
        }

        [Fact]
        public async Task BasicDeferredResultsAsync()
        {
            // setup

            await new Command(_database)
            {
                CreateTable.From<Sample>("deferred_async_table"),
                new Insert {Into = "deferred_async_table", Columns = {"ID", "Column1"}, Values = {1, "Test"}},
            }.ExecuteAsync();

            // test

            await using var context = _database.CreateDeferContext();

            var command1 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_async_table"}
            };

            var command2 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_async_table"}
            };

            var results1 = command1.FetchListDeferred<Sample>();
            var results2 = command2.FetchListDeferred<Sample>();

            Assert.NotEmpty(await results1.ResultAsync);
            Assert.NotEmpty(await results2.ResultAsync);
        }

        [Fact]
        public async Task DeferredContextAddsTransactions()
        {
            // setup

            await new Command(_database)
            {
                CreateTable.From<Sample>("deferred_transaction_async_table"),
                new Insert
                {
                    Into = "deferred_transaction_async_table", Columns = {"ID", "Column1"}, Values = {1, "Test"}
                },
            }.ExecuteAsync();

            // test

            await using var context = _database.CreateDeferContext(
                new BeginTransaction(),
                new CommitTransaction());

            var command1 = new Command(context)
            {
                new Update
                {
                    Table = "deferred_transaction_async_table", Set = {{"Column1", "Test1"}},
                    Where = new Column("ID") == new Constant(1)
                }
            };

            var command2 = new Command(context)
            {
                new Update
                {
                    Table = "deferred_transaction_async_table", Set = {{"Column1", "Test2"}},
                    Where = new Column("ID") == new Constant(1)
                }
            };

            command1.ExecuteDeferred();
            command2.ExecuteDeferred();
        }

        [Fact]
        public async Task DeferredInlineResultsAsync()
        {
            // setup

            await new Command(_database)
            {
                CreateTable.From<Sample>("deferred_inline_async_table"),
                new Insert {Into = "deferred_inline_async_table", Columns = {"ID", "Column1"}, Values = {1, "Test"}},
            }.ExecuteAsync();

            // test

            await using var context = _database.CreateDeferContext();

            var results1 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_inline_async_table"}
            }.FetchListDeferred<Sample>();

            var results2 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_inline_async_table"}
            }.FetchListDeferred<Sample>();

            Assert.NotEmpty(await results1.ResultAsync);
            Assert.NotEmpty(await results2.ResultAsync);
        }

        [Fact]
        public async Task MixedDeferredAsyncAndSync()
        {
            // setup

            CreateTable.From<Sample>("deferred_inline_table");

            await new Command(_database)
            {
                CreateTable.From<Sample>("deferred_mixed_table"),
                new Insert {Into = "deferred_mixed_table", Columns = {"ID", "Column1"}, Values = {1, "Test"}},
            }.ExecuteAsync();

            // test

            await using var context = _database.CreateDeferContext();

            var results1 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_mixed_table"}
            }.FetchListDeferred<Sample>();

            var results2 = new Command(context)
            {
                new Select {Columns = "ID", From = "deferred_mixed_table"}
            }.FetchListDeferred<Sample>();

            Assert.NotEmpty(await results1.ResultAsync);
            Assert.NotEmpty(results2.Result);
        }

        [Fact]
        public void EmptyCommandsAreSkipped()
        {
            var context = _database.CreateDeferContext();
            try
            {
                {
                    new Command(context)
                    {
                        new Update
                        {
                            Table = "invalid_table_name",
                            Set = {{"Column1", new Value("Test1"), false}}
                        }
                    }.ExecuteDeferred();
                }
            }
            finally
            {
                // no error should occur even though the table name is invalid since
                // the update command is empty and will be skipped
                
                context.Dispose();
            }
        }
        
        [Fact]
        public async Task EmptyAsyncCommandsAreSkipped()
        {
            var context = _database.CreateDeferContext();
            try
            {
                {
                    new Command(context)
                    {
                        new Update
                        {
                            Table = "invalid_table_name",
                            Set = {{"Column1", new Value("Test1"), false}}
                        }
                    }.ExecuteDeferred();
                }
            }
            finally
            {
                // no error should occur even though the table name is invalid since
                // the update command is empty and will be skipped
                
                await context.DisposeAsync();
            }
        }
    }
}