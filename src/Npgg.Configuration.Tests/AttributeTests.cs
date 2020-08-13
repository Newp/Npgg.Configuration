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

			var type = typeof(AttributeTestObject);

			var members = type.GetMembers();
				//BindingFlags.GetField| BindingFlags.SetField| BindingFlags.SetProperty| BindingFlags.GetProperty| BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			var ss = members.Select(mem => mem.Name).ToArray();

			foreach (var mem in members)
			{
				var xx = mem.GetCustomAttribute<ConfigColumnAttribute>()?.ColumnName ?? mem.Name;
			}


			//


			string csv =
@"Key,camel_case
0,ok
1,ok
";

			var loaded = loader.Load<AttributeTestObject>(csv);

			Assert.Equal(2, loaded.Count);

			foreach(var item in loaded)
			{
				Assert.Equal("ok", item.CamelCase);
			}

		}

		[Fact]
		public void RequiredTest()
		{

			string csv =
@"Key,//camel_case
0,ok
1,ok
";

			Assert.Throws<RequiredColumnNotFoundException>(()=>loader.Load<AttributeTestObject>(csv));
				
			//Assert.Throws();
			

		}
	}

    



    
}
