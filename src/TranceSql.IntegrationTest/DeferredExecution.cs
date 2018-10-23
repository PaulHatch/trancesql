using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TranceSql.IntegrationTest
{
    public class DeferredExecution : BaseDatabaseTest
    {
        [Fact]
        public void BasicDeferredResults()
        {
            // setup
            
            new Command(_database)
            {
                CreateTable.From<Sample>("deferred_table"),
                new Insert { Into = "deferred_table", Columns = {"ID", "Column1"}, Values = { 1, "Test" } },
            }.Execute();

            // test

            var context = _database.CreateDeferContext();

            var command1 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_table" }
            };

            var command2 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_table" }
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
                new Insert { Into = "deferred_inline_table", Columns = {"ID", "Column1"}, Values = { 1, "Test" } },
            }.Execute();

            // test

            var context = _database.CreateDeferContext();

            var results1 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_inline_table" }
            }.FetchListDeferred<Sample>();

            var results2 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_inline_table" }
            }.FetchListDeferred<Sample>();
            
            Assert.NotEmpty(results1.Result);
            Assert.NotEmpty(results2.Result);
        }

        [Fact]
        public async Task BasicDeferredResultsAsync()
        {
            // setup

            new Command(_database)
            {
                CreateTable.From<Sample>("deferred_async_table"),
                new Insert { Into = "deferred_async_table", Columns = {"ID", "Column1"}, Values = { 1, "Test" } },
            }.Execute();

            // test

            var context = _database.CreateDeferContext();

            var command1 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_async_table" }
            };

            var command2 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_async_table" }
            };

            var results1 = command1.FetchListDeferred<Sample>();
            var results2 = command2.FetchListDeferred<Sample>();

            Assert.NotEmpty(await results1.ResultAsync);
            Assert.NotEmpty(await results2.ResultAsync);
        }

        [Fact]
        public async Task DeferredInlineResultsAsync()
        {
            // setup
            
            new Command(_database)
            {
                CreateTable.From<Sample>("deferred_inline_async_table"),
                new Insert { Into = "deferred_inline_async_table", Columns = {"ID", "Column1"}, Values = { 1, "Test" } },
            }.Execute();

            // test

            var context = _database.CreateDeferContext();

            var results1 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_inline_async_table" }
            }.FetchListDeferred<Sample>();

            var results2 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_inline_async_table" }
            }.FetchListDeferred<Sample>();

            Assert.NotEmpty(await results1.ResultAsync);
            Assert.NotEmpty(await results2.ResultAsync);
        }

        [Fact]
        public async Task MixedDeferredAsyncAndSync()
        {
            // setup

            var t = CreateTable.From<Sample>("deferred_inline_table");

            new Command(_database)
            {
                CreateTable.From<Sample>("deferred_mixed_table"),
                new Insert { Into = "deferred_mixed_table", Columns = {"ID", "Column1"}, Values = { 1, "Test" } },
            }.Execute();

            // test

            var context = _database.CreateDeferContext();

            var results1 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_mixed_table" }
            }.FetchListDeferred<Sample>();

            var results2 = new Command(_database, context)
            {
                new Select { Columns = "ID", From = "deferred_mixed_table" }
            }.FetchListDeferred<Sample>();

            Assert.NotEmpty(await results1.ResultAsync);
            Assert.NotEmpty(results2.Result);
        }

    }
}
