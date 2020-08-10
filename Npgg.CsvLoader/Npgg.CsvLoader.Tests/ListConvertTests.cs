using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Npgg.CsvLoader.Tests
{
    

    public class ListConvertTests
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
            Assert.Equal(5, item.Values.Count);
            Assert.Equal("tag", item.Tag);
        }

        [Fact]
        public void IntListTest()
        {

            var list = loader.Load<sample<int>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Count);
            Assert.Equal("tag", item.Tag);
        }



        public class sample<T>
        {
            public int Key { get; set; }
            public List<T> Values { get; set; }
            public string Tag { get; set; }
        }


    }
}
