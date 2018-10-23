using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranceSql.Sqlite;

namespace TranceSql.IntegrationTest
{
    public abstract class BaseDatabaseTest : IDisposable
    {
        protected class Sample
        {
            public int ID { get; set; }
            public string Column1 { get; set; }
        }

        public BaseDatabaseTest()
        {
            _name = $"{this.GetType().Name.ToLower()}.db";
            if (File.Exists(_name))
            {
                File.Delete(_name);
            }
            _database = new SqliteDatabase($"Data Source={_name}");
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

        protected readonly Database _database;
        private string _name;

        public void Dispose()
        {
            if (File.Exists(_name))
            {
                File.Delete(_name);
            }
        }
    }
}
