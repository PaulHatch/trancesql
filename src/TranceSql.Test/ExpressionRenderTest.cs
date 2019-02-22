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
                new ConditionPair(
                    BooleanOperator.Or,
                    new ConditionPair(
                        BooleanOperator.And,
                        Parameter("P1") == Parameter("P2"),
                        Parameter("P3") == Parameter("P4"), true),
                    Parameter("P5") == Parameter("P6"));

            Assert.Equal("(@P1 = @P2 AND @P3 = @P4) OR @P5 = @P6", sut.ToString());
        }

        [Fact]
        public void RightParentheticalRender()
        {
            var sut =
                new ConditionPair(
                    BooleanOperator.And,
                    Parameter("P1") == Parameter("P2"),
                    new ConditionPair(
                        BooleanOperator.Or,
                        Parameter("P3") == Parameter("P4"),
                        Parameter("P5") == Parameter("P6"),
                        true
                    ));

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

        private static readonly Column A = Column("A");
        private static readonly Column B = Column("B");
        private static readonly Column C = Column("C");
        private static readonly Column D = Column("D");
        private static readonly Column E = Column("E");
        private static readonly Column F = Column("F");
        private static readonly Column G = Column("G");
        private static readonly Column H = Column("H");

        public static IEnumerable<object[]> Expressions()
        {
            yield return new object[] { A > B & C > D | E > F, "A > B AND C > D OR E > F" };
            yield return new object[] { A > B & (C > D | E > F), "A > B AND (C > D OR E > F)" };
            yield return new object[] { A > B | (C > D | (E > F | G > H)), "A > B OR (C > D OR (E > F OR G > H))" };
            yield return new object[] { (A > B | C > D) & (E > F | G > H), "(A > B OR C > D) AND (E > F OR G > H)" };
        }

        [Theory]
        [MemberData(nameof(Expressions))]
        public void RenderConditionsWithCorrectParantheses(ConditionPair condition, string expected)
        {
            Assert.Equal(expected, condition.ToString());
        }
    }
}
