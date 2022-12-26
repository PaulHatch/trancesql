using Xunit;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    public class ValueTupleSupport : IClassFixture<DatabaseFixture>
    {
        protected readonly Database _database;

        public ValueTupleSupport(DatabaseFixture db, ITestOutputHelper helper)
        {
            _database = db.GetDatabase(helper);
        }

        [Fact]
        public void SimpleTuples()
        {
            var sut = new Command(_database)
            {
                new Select
                {
                    Columns = {
                        new Constant(1), Constant.Unsafe("Test")
                    }
                }
            };

            var result = sut.Fetch<(int number, string text)>();

            Assert.Equal(1, result.number);
            Assert.Equal("Test", result.text);
        }

        [Fact]
        public void NineValues()
        {
            // Behind the scenes, automatic tuples with more than 7 values are
            // mapped to ValueTuple<T1...T7, TRest> where TRest is another nested
            // ValueTuple. Make sure we handle this case.

            var sut = new Command(_database)
            {
                new Select
                {
                    Columns = {
                        new Constant(1),
                        new Constant(2),
                        new Constant(3),
                        new Constant(4),
                        new Constant(5),
                        new Constant(6),
                        new Constant(7),
                        new Constant(8),
                        new Constant(9),
                    }
                }
            };

            var result = sut.Fetch<(int one, int two, int three, int four, int five, int six, int seven, int eight, int nine)>();

            Assert.Equal(1, result.one);
            Assert.Equal(2, result.two);
            Assert.Equal(3, result.three);
            Assert.Equal(4, result.four);
            Assert.Equal(5, result.five);
            Assert.Equal(6, result.six);
            Assert.Equal(7, result.seven);
            Assert.Equal(8, result.eight);
            Assert.Equal(9, result.nine);
        }
    }
}
