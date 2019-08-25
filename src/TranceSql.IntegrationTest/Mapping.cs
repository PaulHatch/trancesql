using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TranceSql.Processing;
using Xunit;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    public class Mapping : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public Mapping(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(new TestTracer(helper));
        }

        sealed class TestAttribute : Attribute { public string Format { get; set; } }
        class TestBinder : ICustomBinder
        {
            public bool DoesApply(PropertyInfo property) => property.GetCustomAttribute<TestAttribute>() != null;
            public object MapValue(PropertyInfo property, object value)
            {
                var attr = property.GetCustomAttribute<TestAttribute>();
                return String.Format(attr.Format, value);
            }
        }
        class TestResult
        {
            public string PropertyOne { get; set; }

            [Test(Format = "Value={0}")]
            public string PropertyTwo { get; set; }
        }

        [Fact]
        public async Task CustomBinder()
        {
            EntityMapping.RegisterBinder(new TestBinder());

            var result = await new Command(_database)
            {
                new Select
                {
                    Columns =
                    {
                        Constant.Unsafe("Test").As("PropertyOne"),
                        Constant.Unsafe("Test").As("PropertyTwo")
                    }
                }
            }.FetchAsync<TestResult>();

            Assert.Equal("Test", result.PropertyOne);
            Assert.Equal("Value=Test", result.PropertyTwo);
        }
    }
}
