using OpenTracing.Mock;
using System;
using System.Collections.Generic;
using System.Text;
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
                _outputHelper.WriteLine($"  {tag.Key}: {tag.Value}");
            }
        }
    }
}
