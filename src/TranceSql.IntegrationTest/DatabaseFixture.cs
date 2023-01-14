using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TranceSql.MySql;
using TranceSql.Oracle;
using TranceSql.Postgres;
using TranceSql.Sqlite;
using TranceSql.SqlServer;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    public class DatabaseFixture
    {
        private static char _nameIndex = 'a';
        private static object _nameLocker = new object();
        private Dialect _dialect;
        private string _dbName;
        private bool _createdDatabase;
        private Database _masterDatabase; // used for fixture operations (create/drop test database)

        private Func<Database> _getDatabase;
        private Database _database;
        private ActivitySource _activitySource;

        public Database GetDatabase(ITestOutputHelper outputHelper)
        {
            if (_database != null) return _database;
            RegisterActivityOutput(outputHelper);
            _database = _getDatabase();
            return _database;
        }

        
        protected enum Dialect
        {
            MySql,
            SqlServer,
            Oracle,
            Postgres,
            Sqlite
        }

        private void RegisterActivityOutput(ITestOutputHelper outputHelper)
        {
            ActivitySource.AddActivityListener(new ActivityListener
            {
                ShouldListenTo = a => a == _activitySource,
                Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStarted = activity =>
                {
                    var started = $"Started '{activity.OperationName}'";
                    Console.WriteLine(started);
                    outputHelper.WriteLine(started);
                    foreach (var tag in activity.Tags)
                    {
                        var output = $"  {tag.Key}: {tag.Value}";
                        Console.WriteLine(output);
                        outputHelper.WriteLine(output);    
                    }
                    
                },
                ActivityStopped = activity =>
                {
                    var stopped = $"Stopped '{activity.OperationName}'";
                    Console.WriteLine(stopped);
                    outputHelper.WriteLine(stopped);
                }
            });
        }

        public DatabaseFixture()
        {

            lock (_nameLocker)
            {
                _nameIndex++;
                _dbName = $"integration{_nameIndex}";
            }
            
            _activitySource = new ActivitySource("TranceSql.IntegrationTest");

            var dialect = Environment.GetEnvironmentVariable("DIALECT") ?? "Sqlite";
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Data Source=:memory:;Version=3;New=True;";
            if (!Enum.TryParse(dialect, true, out _dialect))
            {
                Console.WriteLine($"Warning, could not resolve DIALECT={dialect} to known dialect.");
                _dialect = Dialect.Sqlite;
            }

            switch (_dialect)
            {
                case Dialect.MySql:
                    WaitForDatabase(new MySqlDatabase(connectionString));
                    _getDatabase = () => new MySqlDatabase(connectionString + $";Database={_dbName}", _activitySource);
                    break;
                case Dialect.Oracle:
                    WaitForDatabase(new OracleDatabase(connectionString));
                    _getDatabase = () => new OracleDatabase(connectionString + $";Database={_dbName}", _activitySource);
                    break;
                case Dialect.Postgres:
                    WaitForDatabase(new PostgresDatabase(connectionString));
                    _getDatabase = () => new PostgresDatabase(connectionString + $";Database={_dbName}", _activitySource);
                    break;
                case Dialect.SqlServer:
                    WaitForDatabase(new SqlServerDatabase(connectionString));
                    _getDatabase = () => new SqlServerDatabase(connectionString + $";Database={_dbName}", _activitySource);
                    break;
                case Dialect.Sqlite:
                default:
                    if (File.Exists($"{_dbName}.db")) { File.Delete($"{_dbName}.db"); }
                    _getDatabase = () => new SqliteDatabase($"Data Source={_dbName }.db", _activitySource);
                    break;
            }

            try
            {
                new Command(_getDatabase())
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                        new Command(database) { new CreateDatabase(_dbName) }.Execute();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR CREATING DATABASE:");
                        Console.WriteLine(e);
                    }

                    _createdDatabase = true;
                    _masterDatabase = database;

                    Thread.Sleep(2000);
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
                case Dialect.SqlServer:
                case Dialect.Oracle:
                case Dialect.Postgres:
                    if (_createdDatabase)
                    {
                        try
                        {
                            new Command(_masterDatabase) {
                            new Raw("DROP DATABASE " + _dbName)
                        }.Execute();
                        }
                        catch { }
                    }
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
