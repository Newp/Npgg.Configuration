using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Npgg.CsvLoader.Tests
{
    public class ArrayConvertTests
    {
        readonly SimpleLoader loader = new SimpleLoader();

        string csv = @"Key,Values,Tag
1,""1,2,3,4,5"",tag";
        [Fact]
        public void StringListTest()
        {

            var list = loader.Load<sample<string>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Length);
            Assert.Equal("tag", item.Tag);
        }

        [Fact]
        public void IntListTest()
        {

            var list = loader.Load<sample<int>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Length);
            Assert.Equal("tag", item.Tag);
        }


        [Fact]
        public void Test()
        {
            //var converter = new ArrayConverter();

            //var xx= converter.
        }

        public class sample<T>
        {
            public int Key { get; set; }
            public T[] Values { get; set; }
            public string Tag { get; set; }
        }


    }
}
