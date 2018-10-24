using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranceSql.MySql;
using TranceSql.Oracle;
using TranceSql.Postgres;
using TranceSql.Sqlite;
using TranceSql.SqlServer;

namespace TranceSql.IntegrationTest
{
    public abstract class BaseDatabaseTest : IDisposable
    {
        protected enum Dialect
        {
            MySql,
            SqlServer,
            Oracle,
            Postgres,
            Sqlite
        }

        protected class Sample
        {
            public int ID { get; set; }
            public string Column1 { get; set; }
        }

        protected readonly Database _database;
        private string _dbName;
        private Dialect _dialect;

        public BaseDatabaseTest()
        {
            _dbName = this.GetType().Name.ToLower();
            var dialect = Environment.GetEnvironmentVariable("DIALECT");
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (!Enum.TryParse<Dialect>(dialect, true, out _dialect))
            {
                _dialect = Dialect.Sqlite;
            }

            switch (_dialect)
            {
                case Dialect.MySql:
                    break;
                case Dialect.Oracle:
                    break;
                case Dialect.Postgres:
                    break;
                case Dialect.SqlServer:
                    new Command(new SqlServerDatabase(connectionString)) { new CreateDatabase(_dbName) }.Execute();
                    _database = new SqlServerDatabase(connectionString + $";Database={_dbName}");
                    break;
                case Dialect.Sqlite:
                default:
                    if (File.Exists(_dbName)) { File.Delete(_dbName); }
                    _database = new SqliteDatabase($"Data Source={_dbName }.db");
                    break;
            }

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


        public void Dispose()
        {
            switch (_dialect)
            {
                case Dialect.MySql:
                    break;
                case Dialect.SqlServer:
                    break;
                case Dialect.Oracle:
                    break;
                case Dialect.Postgres:
                    break;
                case Dialect.Sqlite:
                    if (File.Exists($"{_dbName}.db")) { File.Delete($"{_dbName}.db"); }
                    break;
                default:
                    break;
            }
        }
    }
}
