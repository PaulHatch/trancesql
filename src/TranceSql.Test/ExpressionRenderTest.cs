using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static TranceSql.UsingStatic;

namespace TranceSql.Test
{
    public class ExpressionRenderTest
    {
        [Fact]
        public void AddExpressionRender()
        {
            var sut = Column("Column1") + Column("Column2");

            Assert.Equal("Column1 + Column2", sut.ToString());
        }

        [Fact]
        public void DivideExpressionRender()
        {
            var sut = Column("Column1") / Column("Column2");

            Assert.Equal("Column1 / Column2", sut.ToString());
        }

        [Fact]
        public void EqualExpressionRender()
        {
            var sut = Column("Column1") == Column("Column2");

            Assert.Equal("Column1 = Column2", sut.ToString());
        }

        [Fact]
        public void ComplexExpressionRender()
        {
            var sut = Column("Column1") + Parameter("P1") == Column("Column2");

            Assert.Equal("Column1 + @P1 = Column2", sut.ToString());
        }

        [Fact]
        public void NoParentheticalRender()
        {
            var sut = Parameter("P1") == Parameter("P2") &
                      Parameter("P3") == Parameter("P4") |
                      Parameter("P5") == Parameter("P6");

            Assert.Equal("@P1 = @P2 AND @P3 = @P4 OR @P5 = @P6", sut.ToString());
        }

        [Fact]
        public void LeftParentheticalRender()
        {
            var sut =
                Condition.Nested(
                    Parameter("P1") == Parameter("P2") &
                    Parameter("P3") == Parameter("P4")) |
                Parameter("P5") == Parameter("P6");

            Assert.Equal("(@P1 = @P2 AND @P3 = @P4) OR @P5 = @P6", sut.ToString());
        }

        [Fact]
        public void RightParentheticalRender()
        {
            var sut =
                Parameter("P1") == Parameter("P2") &
                Condition.Nested(
                    Parameter("P3") == Parameter("P4") |
                    Parameter("P5") == Parameter("P6"));

            Assert.Equal("@P1 = @P2 AND (@P3 = @P4 OR @P5 = @P6)", sut.ToString());
        }
        [Fact]
        public void IfRender()
        {
            var sut = new If(Value(1) == Value(2))
            {
                Then = new StatementBlock { new Select { Columns = { new Constant(1) } } },
                Else = new Select { Columns = { new Constant(2) } }
            };

            var result = sut.ToString();

            Assert.Equal("IF (@P1 = @P2)\nBEGIN\nSELECT 1;\nEND\nELSE\nSELECT 2;", result);
        }
    }
}
