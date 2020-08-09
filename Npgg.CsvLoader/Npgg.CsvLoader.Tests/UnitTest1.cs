using System;
using Xunit;
using Npgg.CsvLoader;
using System.Linq;

namespace Npgg.CsvParser.Tests
{
    public class UnitTest1
    {


        [Fact]
        public void Test1()
        {
            
        }

        

        string MakeRow (params object[] values)=> string.Join(',', values);

        string MakeQuotationMarkedRow(params object[] values) => string.Join(',', values.Select(value=> $"\"{value}\""));

        [Fact]
        public void TestValueCreateTest1()
        {
            var generated = MakeQuotationMarkedRow(1, 2);

            Assert.Equal("\"1\",\"2\"", generated);
        }
        [Fact]
        public void TestValueCreateTest2()
        {
            var generated = MakeRow(1, 2, 3,4);

            Assert.Equal("1,2,3,4", generated);
        }
    }



    
}
