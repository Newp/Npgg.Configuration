using Xunit;
using System.Linq;
using Npgg.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Npgg.Tests
{
	public class CustomConverterTests
	{


		[Fact]
		public void CustomConverterTest()
		{
			var csvString = @"Key,Vector3
1,""(1,2,3)""
";

			CustomConverter<Vector3>.RegistConverter<Vector3Converter>();

			CsvLoader loader = new CsvLoader();

			var list = loader.Load<Vector3Row>(csvString);

			Assert.Single(list);

			var item = list.FirstOrDefault();
			Assert.NotNull(item);
			Assert.Equal(1, item.Key);
			Assert.Equal(1, item.Vector3.X);
			Assert.Equal(2, item.Vector3.Y);
			Assert.Equal(3, item.Vector3.Z);
		}

		
		class Vector3Row
		{
			public int Key { get; set; }
		
			public Vector3 Vector3 { get; set; }
		}

		class Vector3
		{
			public float X, Y, Z;
		}

		class Vector3Converter : CustomConverter<Vector3>
		{
			public override Vector3 Convert(string value) //input (1,2,3)
			{
				var splited = value.Split(',')
					.Select(d => d.Trim('(', ')')) // array["1", "2", "3"]
					.Select(d => float.Parse(d)).ToArray(); // array[1, 2, 3]

				return new Vector3() { X = splited[0], Y = splited[1], Z = splited[2] };

			}
		}
		class DictionaryConverter : CustomConverter<Dictionary<string, byte[]>>
		{
			public override Dictionary<string, byte[]> Convert(string value) //input (1,2,3)
			{
				return JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(value);
			}
		}



		class Dictionarys
		{
			public Dictionary<string, byte[]> Value { get; set; }
		}

		[Fact]
		public void DictionaryMemberConverterTest()
		{
			var dic = new Dictionary<string, byte[]>()
			{
				{ "aa", new byte[] {1, 2, 3 } }
			};

			var json = JsonConvert.SerializeObject(dic);

			var csvString = "Value\n" + json;

			CustomConverter<Dictionary<string, byte[]>>.RegistConverter<DictionaryConverter>();

			CsvLoader loader = new CsvLoader();

			var list = loader.Load<Dictionarys>(csvString);

		}
	}

    



    
}
