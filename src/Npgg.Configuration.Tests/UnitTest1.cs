using System;
using Xunit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgg.Configuration;

namespace Npgg.Tests
{

	public class UnitTest1
	{

		CsvLoader loader = new CsvLoader();
		public class SampleObject
		{
			public int Key { get; set; }
			public string Value;
		}

		string MakeRow(params object[] values) => string.Join(',', values);

		string MakeQuotationMarkedRow(params object[] values) => string.Join(',', values.Select(value => $"\"{value}\""));

		[Fact]
		public void CustomConverterTest()
		{

		}

		[Fact]
		public void TestValueCreateTest1()
		{
			var generated = MakeQuotationMarkedRow(1, 2);

			Assert.Equal("\"1\",\"2\"", generated);
		}
		[Fact]
		public void TestValueCreateTest2()
		{
			var generated = MakeRow(1, 2, 3, 4);

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

			var loaded = loader.Load<SampleObject>(csv);

			Assert.Equal(2, loaded.Count);

			for (int i = 0; i < 2; i++)
			{
				var item = loaded[i];

				Assert.Equal(i, item.Key);
				Assert.Equal("Value" + i, item.Value);
			}
		}


		[Fact]
		public void RowTest()
		{
			string csv =
@"Key,Value
1,Value1
#2,Value2
2,Value2
";

			var loaded = loader.Load<SampleObject>(csv);

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
		public void ColumnTest()//
		{
			string csv =
@"Key,#Value
0,Value0
1,Value1
";

			var loaded = loader.Load<SampleObject>(csv);

			Assert.Equal(2, loaded.Count);

			for (int i = 0; i < 2; i++)
			{
				var item = loaded[i];

				Assert.Equal(i, item.Key);
				Assert.Null(item.Value);
			}
		}

		[Fact]
		public void InvalidValueConvertTest()//
		{
			string csv =
@"Key,#Value
1,Value0
bbbb,Value1
";

			var exception = Assert.Throws<ConvertException>(() => loader.Load<SampleObject>(csv));

			Assert.Equal(nameof(SampleObject.Key), exception.ColumnName);
			Assert.Equal(2, exception.LineNumber);
			Assert.Equal("bbbb", exception.TextValue);

		}
	}

    
}
