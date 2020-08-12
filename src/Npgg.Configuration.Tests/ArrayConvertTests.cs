using Npgg.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Npgg.Tests
{
    public class ArrayConvertTests
    {
        readonly CsvLoader loader = new CsvLoader();

        string csv = @"Key ,Values ,Tag
1,""1,2,3,4,5"",tag ";
        [Fact]
        public void StringListTest()
        {

            var list = loader.Load<ArraySample<string>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Length);
            Assert.Equal("tag", item.Tag);
        }

        [Fact]
        public void IntListTest()
        {

            var list = loader.Load<ArraySample<int>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Length);
            Assert.Equal("tag", item.Tag);
        }

		[Fact]
		public void TsvTest()
		{
			string pattern = "\t(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
			TsvLoader tsv = new TsvLoader();

			string input = "a	\"b1	b2\"	c";
			var ss2 = Regex.Split(input, pattern);
			var sss = tsv.Split(input);

			Assert.Equal(3, sss.Length);
		}


        [Fact]
        public void Test()
        {
            //var converter = new ArrayConverter();

            //var xx= converter.
        }

        public class ArraySample<T>
        {
            public int Key { get; set; }
            public T[] Values { get; set; }
            public string Tag { get; set; }
        }


    }
}
