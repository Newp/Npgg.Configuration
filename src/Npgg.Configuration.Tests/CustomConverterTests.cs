using Xunit;
using System.Linq;
using Npgg.Configuration;

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
	}

    



    
}
