using System;
using System.Collections.Generic;
using System.Text;
using TranceSql.Language;
using Xunit;
using static TranceSql.Language.UsingStatic;

namespace TranceSql.Test
{
    public class InsertRenderingTest
    {
        [Fact]
        public void BasicInsertRender()
        {
            var sut = new Insert
            {
                Into = "Table",
                Columns = "Column1",
                Values = { Value(123) }
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table (Column1)\nVALUES (@P1);", result);
        }

        [Fact]
        public void InsertMultipleSingleValuesRender()
        {
            var sut = new Insert
            {
                Into = "Table",
                Columns = "Column1",
                Values = {
                    { Value(123) },
                    { Value(123)  }
                }
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table (Column1)\nVALUES (@P1),\n(@P2);", result);
        }

        [Fact]
        public void InsertMultipleMultiValuesRender()
        {
            var sut = new Insert
            {
                Into = "Table",
                Columns = { "Column1", "Column2" },
                Values = {
                    { Value(123), Value(123) },
                    { Value(123), Value(123) }
                }
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table (Column1, Column2)\nVALUES (@P1, @P2),\n(@P3, @P4);", result);
        }

        [Fact]
        public void InsertFromSelectRender()
        {
            var sut = new Insert
            {
                Into = "Table",
                Columns = { "Column1", "Column2" },
                Values = new Select
                {
                    Columns = { "Column1", "Column2" },
                    From = "Table2"
                }
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table (Column1, Column2)\n(SELECT Column1, Column2\nFROM Table2);", result);
        }

        [Fact]
        public void InsertWithoutColumnsRender()
        {
            var sut = new Insert
            {
                Into = "Table",
                Values = {
                    { Value(123), Value(123) }
                }
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table\nVALUES (@P1, @P2);", result);
        }

        [Fact]
        public void InsertWithoutColumnsMultipleRowsRender()
        {
            var sut = new Insert
            {
                Into = "Table",
                Values = {
                    Values(123,123),
                    Values(123,123)
                }
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table\nVALUES (@P1, @P2),\n(@P3, @P4);", result);
        }

        [Fact]
        public void InsertDefaultValuesRender()
        {
            var sut = new Insert
            {
                Into = "Table"
            };

            var result = sut.ToString();

            Assert.Equal("INSERT INTO Table\nDEFAULT VALUES;", result);
        }
    }
}
