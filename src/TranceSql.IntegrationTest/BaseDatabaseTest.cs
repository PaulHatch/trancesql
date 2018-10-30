using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    public abstract class BaseDatabaseTest : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public BaseDatabaseTest(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(new TestTracer(helper));
        }
    }
}
