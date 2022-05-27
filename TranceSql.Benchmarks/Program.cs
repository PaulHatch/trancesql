using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TranceSql;
using static Schema;

// This benchmark is used to gauge the performance impact of using command objects to
// generate queries compared to using a string builder. The string builder code is not
// a perfect representation of the command object code, it is just an approximation for
// the sake of comparison.

BenchmarkRunner.Run(typeof(RenderingBenchmarks).Assembly);

// RESULTS

//  |              Method |       Mean |   Error |  StdDev |  Gen 0 | Allocated |
//  |-------------------- |-----------:|--------:|--------:|-------:|----------:|
//  |      SelectRendered |   790.8 ns | 1.05 ns | 0.93 ns | 0.9832 |   2,056 B |
//  | SelectStringBuilder |   403.6 ns | 0.99 ns | 0.88 ns | 0.6309 |   1,320 B |
//  |      UpdateRendered | 1,205.0 ns | 1.28 ns | 1.20 ns | 1.2150 |   2,544 B |
//  | UpdateStringBuilder |   254.8 ns | 0.23 ns | 0.20 ns | 0.3939 |     824 B |
//  |      InsertRendered | 1,068.9 ns | 1.19 ns | 1.06 ns | 0.9518 |   1,992 B |
//  | InsertStringBuilder |   242.7 ns | 0.29 ns | 0.26 ns | 0.3939 |     824 B |
//  |      DeleteRendered |   453.9 ns | 0.81 ns | 0.76 ns | 0.6309 |   1,320 B |
//  | DeleteStringBuilder |   126.9 ns | 0.37 ns | 0.31 ns | 0.2332 |     488 B |

// Relative Memory Utilization Commands vs String Builder 
//  Select: 155.8 %
//  Update: 308.7 %
//  Insert: 241.7 %
//  Delete: 270.5 %
//  Average: 244.2 %

[MemoryDiagnoser]
public class RenderingBenchmarks
{
    [Benchmark]
    public int SelectRendered()
    {
        var sql = new Select
        {
            Columns = {Sample.Name, Sample.Age, Sample.Created},
            From = Sample,
            Where = Sample.Age > new Value(10)
        }.ToString();

        return sql.Length;
    }
    
    [Benchmark]
    public int SelectStringBuilder()
    {
        var sb = new StringBuilder();
        sb.Append("SELECT ");
        sb.Append(Sample.Name.Name);
        sb.Append(", ");
        sb.Append(Sample.Age.Name);
        sb.Append(", ");
        sb.Append(Sample.Created.Name);
        sb.Append(" FROM ");
        sb.Append(((Table)Sample).Schema);
        sb.Append('.');
        sb.Append(((Table)Sample).Name);
        sb.Append(" WHERE ");
        sb.Append(Sample.Age.Name);
        sb.Append(" > ");
        sb.Append(new Value(10));

        var sql = sb.ToString();
        return sql.Length;
    }

    [Benchmark]
    public int UpdateRendered()
    {
        var sql = new Update
        {
            Table = Sample,
            Set =
            {
                {Sample.Name, new Value(1)},
                {Sample.Age, new Value(1)},
                {Sample.Created, new Value(1)}
            },
            Where = Sample.Age > new Value(10)
        }.ToString();

        return sql.Length;
    }

    [Benchmark]
    public int UpdateStringBuilder()
    {
        var sb = new StringBuilder();
        sb.Append("INSERT INTO ");
        sb.Append(((Table) Sample).Schema);
        sb.Append('.');
        sb.Append(((Table) Sample).Name);
        sb.Append(" (");
        sb.Append(Sample.Name.Name);
        sb.Append(", ");
        sb.Append(Sample.Age.Name);
        sb.Append(", ");
        sb.Append(Sample.Created.Name);
        sb.Append(") VALUES (");
        sb.Append(1);
        sb.Append(", ");
        sb.Append(1);
        sb.Append(", ");
        sb.Append(1);
        sb.Append(')');
        
        var sql = sb.ToString();
        return sql.Length;
    }

    [Benchmark]
    public int InsertRendered()
    {
        var sql = new Insert
        {
            Into = Sample,
            Columns = {Sample.Name, Sample.Age, Sample.Created},
            Values = {new Value(1), new Value(1), new Value(1)}
        }.ToString();

        return sql.Length;
    }

    [Benchmark]
    public int InsertStringBuilder()
    {
        var sb = new StringBuilder();
        sb.Append("INSERT INTO ");
        sb.Append(((Table)Sample).Schema);
        sb.Append('.');
        sb.Append(((Table)Sample).Name);
        sb.Append(" (");
        sb.Append(Sample.Name.Name);
        sb.Append(", ");
        sb.Append(Sample.Age.Name);
        sb.Append(", ");
        sb.Append(Sample.Created.Name);
        sb.Append(") VALUES (");
        sb.Append(1);
        sb.Append(", ");
        sb.Append(1);
        sb.Append(", ");
        sb.Append(1);
        sb.Append(')');
        
        var sql = sb.ToString();
        return sql.Length;
    }
    
    [Benchmark]
    public int DeleteRendered()
    {
        var sql = new Delete
        {
            From = Sample,
            Where = Sample.Age > new Value(10)
        }.ToString();

        return sql.Length;
    }
    
    [Benchmark]
    public int DeleteStringBuilder()
    {
        var sb = new StringBuilder();

        sb.Append("DELETE FROM ");
        sb.Append(((Table)Sample).Schema);
        sb.Append('.');
        sb.Append(((Table)Sample).Name);
        sb.Append(" WHERE ");
        sb.Append(Sample.Age.Name);
        sb.Append(" > ");
        sb.Append('?');

        var sql = sb.ToString();
        return sql.Length;
    }
}

public static class Schema
{
    public static SampleTable Sample { get; } = new();

    public class SampleTable : TableSchema
    {
        public SampleTable() : base("public", "sample")
        {
        }

        public Column Name => Column("name");
        public Column Age => Column("age");
        public Column Created => Column("created_at");
    }
}