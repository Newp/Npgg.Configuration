using System;
using Xunit;
using Npgg.CsvLoader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Fact]
        public void CsvLoadTest()
        {
            string csv =
@"Key,Value
0,Value0
1,Value1
";
            SimpleLoader loader = new SimpleLoader();

            var loaded = loader.Load<CsvSample>(csv);

            Assert.Equal(2, loaded.Count);

            for (int i = 0; i < 2; i++)
            {
                var item = loaded[i];

                Assert.Equal(i, item.Key);
                Assert.Equal("Value" + i, item.Value);
            }
        }


        [Fact]
        public void Row저쩌구()
        {
            string csv =
@"Key,Value
1,Value1
#2,Value2
2,Value2
";
            SimpleLoader loader = new SimpleLoader();

            var loaded = loader.Load<CsvSample>(csv);

            Assert.Equal(2, loaded.Count);

            {
                var item = loaded.First();

                Assert.Equal(1, item.Key);
                Assert.Equal("Value1", item.Value);
            }
            {
                var item = loaded.Last();

                Assert.Equal(2, item.Key);
                Assert.Equal("Value2", item.Value);
            }
        }

        [Fact]
        public void Column어쩌구()
        {
            string csv =
@"Key,#Value
0,Value0
1,Value1
";
            SimpleLoader loader = new SimpleLoader();

            var loaded = loader.Load<CsvSample>(csv);

            Assert.Equal(2, loaded.Count);

            for (int i = 0; i < 2; i++)
            {
                var item = loaded[i];

                Assert.Equal(i, item.Key);
                Assert.Null(item.Value);
            }
        }

        public class CsvSample
        {
            public int Key { get; set; }
            public string Value;
        }
    }

    



    
}
