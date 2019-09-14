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


        enum TestLongEnum : long { One = 1, Two = 2 }
        enum TestIntEnum : int { One = 1, Two = 2 }
        enum TestShortEnum : short { One = 1, Two = 2 }
        enum TestByteEnum : byte { One = 1, Two = 2 }

        [Theory]
        [InlineData((long)2)]
        [InlineData((int)2)]
        [InlineData((short)2)]
        [InlineData((byte)2)]
        public async Task EnumByValue(object value)
        {
            var command = new Command(_database)
            {
                new Select { Columns = new Value(value) }
            };

            Assert.Equal(TestLongEnum.Two, await command.FetchAsync<TestLongEnum>());
            Assert.Equal(TestIntEnum.Two, await command.FetchAsync<TestIntEnum>());
            Assert.Equal(TestShortEnum.Two, await command.FetchAsync<TestShortEnum>());
            Assert.Equal(TestByteEnum.Two, await command.FetchAsync<TestByteEnum>());
        }
    }
}
