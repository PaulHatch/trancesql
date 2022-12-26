using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TranceSql.Processing;
using Xunit;
using Moq;

namespace TranceSql.Test;

public class EntityMappingTest
{
    class TestClass
    {
        public IDictionary<string, string> IDictionary { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>();
        public IList<string> IList { get; } = new List<string>();
        public List<string> List { get; } = new List<string>();
    }
    private readonly Dictionary<string, int> _map = new ()
    {
        {"IDictionary", 0},
        {"Dictionary", 1},
        {"IList", 2},
        {"List", 3}
    };
    
    [Fact]
    public void CanMapGetOnlyDictionary()
    {
        var create = EntityMapping.GetEntityFunc<TestClass>();
        var readerMock = new Mock<DbDataReader>();
        readerMock.Setup(m => m.FieldCount).Returns(4);
        readerMock.Setup(m => m.GetName(0)).Returns(nameof(TestClass.IDictionary));
        readerMock.Setup(m => m.GetName(1)).Returns(nameof(TestClass.Dictionary));
        readerMock.Setup(m => m.GetName(2)).Returns(nameof(TestClass.IList));
        readerMock.Setup(m => m.GetName(3)).Returns(nameof(TestClass.List));
            
        readerMock.Setup(m => m.GetFieldType(0)).Returns(typeof(IDictionary<string, string>));
        readerMock.Setup(m => m.GetFieldType(1)).Returns(typeof(Dictionary<string, string>));
        readerMock.Setup(m => m.GetFieldType(2)).Returns(typeof(IList<string>));
        readerMock.Setup(m => m.GetFieldType(3)).Returns(typeof(List<string>));

        var dict = new Dictionary<string, string> {{"name", "value"}};
        var list = new List<string> { "value" };

        readerMock.Setup(m => m.GetOrdinal("First")).Returns(0);
        readerMock.Setup(m => m.GetValue(0)).Returns(dict);
        readerMock.Setup(m => m.GetValue(1)).Returns(dict);
        readerMock.Setup(m => m.GetValue(2)).Returns(list);
        readerMock.Setup(m => m.GetValue(3)).Returns(list);

        readerMock.SetupSequence(m => m.Read())
            .Returns(true)
            .Returns(false);

        var result = create(readerMock.Object, _map);

        Assert.Equal(1, result.IDictionary.Count);
        Assert.Single(result.Dictionary);
        Assert.Single(result.List);
        Assert.Equal(1, result.IList.Count);
    }
}