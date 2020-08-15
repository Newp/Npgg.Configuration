using Xunit;
using System.Linq;
using System.Reflection;
using Npgg.Configuration;

namespace Npgg.Tests
{
	public class AttributeTests
	{

		CsvLoader loader = new CsvLoader();

		public class AttributeTestObject
		{
			[ConfigColumn] public int Key { get; set; }
			[ConfigColumn("camel_case")] public string CamelCase { get; set; }
		}

		[Fact]
		public void AttributesTest()
		{
			string csv =
@"Key,camel_case
0,ok
1,ok
";
			var loaded = loader.Load<AttributeTestObject>(csv);

			Assert.Equal(2, loaded.Count);

			foreach (var item in loaded)
			{
				Assert.Equal("ok", item.CamelCase);
			}

		}

		[Fact]
		public void RequiredTest2()
		{
			string csv =@"jjjKey";

			var ex = Assert.Throws<RequiredColumnNotFoundException>(() => loader.Load<AttributeTestObject>(csv));

			Assert.Contains(nameof(AttributeTestObject.Key), ex.Message);
		}

		[Fact]
		public void RequiredTest1()
		{

			string csv =
@"Key,//camel_case
0,ok
1,ok
";

			var ex = Assert.Throws<RequiredColumnNotFoundException>(()=>loader.Load<AttributeTestObject>(csv));

			Assert.Contains("camel_case", ex.Message);
				
			//Assert.Throws();
			

		}
	}

    



    
}
