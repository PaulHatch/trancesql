using OpenTracing;
using OpenTracing.Mock;
using OpenTracing.Propagation;
using OpenTracing.Util;
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
    public class DatabaseFixture
    {
        private static int _nameIndex = 0;
        private static object _nameLocker = new object();
        private Dialect _dialect;
        private string _dbName;
        public Func<ITracer, Database> GetDatabase { get; }

        protected enum Dialect
        {
            MySql,
            SqlServer,
            Oracle,
            Postgres,
            Sqlite
        }

        public DatabaseFixture()
        {
            lock (_nameLocker)
            {
                _nameIndex++;
                _dbName = $"integration{_nameIndex}";
            }

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
                    WaitForDatabase(new MySqlDatabase(connectionString));
                    GetDatabase = t => new MySqlDatabase(connectionString + $";Database={_dbName}", t);
                    break;
                case Dialect.Oracle:
                    WaitForDatabase(new OracleDatabase(connectionString));
                    GetDatabase = t => new OracleDatabase(connectionString + $";Database={_dbName}", t);
                    break;
                case Dialect.Postgres:
                    WaitForDatabase(new PostgresDatabase(connectionString));
                    GetDatabase = t => new PostgresDatabase(connectionString + $";Database={_dbName}", t);
                    break;
                case Dialect.SqlServer:
                    WaitForDatabase(new SqlServerDatabase(connectionString));
                    GetDatabase = t => new SqlServerDatabase(connectionString + $";Database={_dbName}", t);
                    break;
                case Dialect.Sqlite:
                default:
                    if (File.Exists($"{_dbName}.db")) { File.Delete($"{_dbName}.db"); }
                    GetDatabase = t => new SqliteDatabase($"Data Source={_dbName }.db", t);
                    break;
            }

            new Command(GetDatabase(new MockTracer()))
            {
                new CreateTable("sample")
                {
                    Columns =
                    {
                        { "id", SqlType.From<int>(), new PrimaryKeyConstraint() },
                        { "column1", SqlType.From<string>(true, 200) }
                    }
                }
            }.Execute();
        }
        
        public void WaitForDatabase(Database database)
        {
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    new Command(database) { new Select { Columns = new Constant(1) } }.Execute();

                    try
                    {
                        new Command(database) {
                            new CreateDatabase(_dbName)
                        }.Execute();
                    }
                    catch { }

                    return;
                }
                catch
                {
                    Thread.Sleep(3000);
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
