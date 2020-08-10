using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Npgg.Tests
{
    

    public class ListConvertTests
    {
        readonly CsvLoader loader = new CsvLoader();

        string csv = @"Key,Values,Tag
1,""1,2,3,4,5"",tag";
        [Fact]
        public void StringListTest()
        {

            var list = loader.Load<ListSample<string>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Count);
            Assert.Equal("tag", item.Tag);
        }

        [Fact]
        public void IntListTest()
        {

            var list = loader.Load<ListSample<int>>(csv);

            Assert.Single(list);

            var item = list.Last();

            Assert.Equal(1, item.Key);
            Assert.Equal(5, item.Values.Count);
            Assert.Equal("tag", item.Tag);
        }



        public class ListSample<T>
        {
            public int Key { get; set; }
            public List<T> Values { get; set; }
            public string Tag { get; set; }
        }


    }
}
