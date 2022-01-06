using OpenTracing.Mock;
using System;
using Xunit.Abstractions;

namespace TranceSql.IntegrationTest
{
    public class TestTracer : MockTracer
    {
        private ITestOutputHelper _outputHelper;

        public TestTracer(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        protected override void OnSpanFinished(MockSpan mockSpan)
        {
            base.OnSpanFinished(mockSpan);

            _outputHelper.WriteLine($"TRACE {mockSpan.OperationName}:");
            foreach (var tag in mockSpan.Tags)
            {
                var output = $"  {tag.Key}: {tag.Value}";
                Console.WriteLine(output);
                _outputHelper.WriteLine(output);
            }
        }
    }
}
