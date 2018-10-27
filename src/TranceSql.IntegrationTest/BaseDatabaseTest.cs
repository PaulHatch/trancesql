using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
                Console.WriteLine($"Warning, could not resolve DIALECT={dialect} to known dialect.");
                _dialect = Dialect.Sqlite;
            }

            switch (_dialect)
            {
                case Dialect.MySql:
                    _database = new MySqlDatabase(connectionString);
                    WaitForDatabase();
                    _database = new MySqlDatabase(connectionString + $";Database={_dbName}");
                    break;
                case Dialect.Oracle:
                    _database = new OracleDatabase(connectionString);
                    WaitForDatabase();
                    _database = new OracleDatabase(connectionString + $";Database={_dbName}");
                    break;
                case Dialect.Postgres:
                    _database = new PostgresDatabase(connectionString);
                    WaitForDatabase();
                    _database = new PostgresDatabase(connectionString + $";Database={_dbName}");
                    break;
                case Dialect.SqlServer:
                    _database = new SqlServerDatabase(connectionString);
                    WaitForDatabase();
                    _database = new SqlServerDatabase(connectionString + $";Database={_dbName}");
                    break;
                case Dialect.Sqlite:
                default:
                    if (File.Exists(_dbName)) { File.Delete(_dbName); }
                    _database = new SqliteDatabase($"Data Source={_dbName }.db");
                    break;
            }

            Console.WriteLine($"Running {_dbName} on {dialect} ({_dialect})");

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

        public void WaitForDatabase()
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    new Command(_database) { new Select { Columns = new Constant(1) } }.Execute();

                    try
                    {
                        new Command(_database) {
                            new CreateDatabase(_dbName)
                        }.Execute();
                    }
                    catch { }

                    return;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
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
