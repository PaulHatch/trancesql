using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace TranceSql.IntegrationTest
{
    public abstract class BaseDatabaseTest : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public BaseDatabaseTest(DatabaseFixture db)
        {
            _database = db.Database;
        }
    }
}
