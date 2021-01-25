using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Npgg.Configuration.Tests
{
	
	public class ObjectToCsvTsvStringTests
	{
		CsvLoader loader = new CsvLoader();
		[Fact]
		public void ToCsvTest()
		{
			var csv = loader.ToCsvString(new[] {
				new Sample() { A = "aaa", B = 1333 },
				new Sample() { A = "bbb", B = 3939 },
			});

			var expect = "A,B\n\"aaa\",\"1333\"\n\"bbb\",\"3939\"";

			Assert.Equal(expect, csv);
		}


		[Fact]
		public void ToTsvTest()
		{
			var csv = loader.ToTsvString(new[] {
				new Sample() { A = "aaa", B = 1333 },
				new Sample() { A = "bbb", B = 3939 },
			});

			var expect = "A\tB\n\"aaa\"\t\"1333\"\n\"bbb\"\t\"3939\"";

			Assert.Equal(expect, csv);
		}

		class Sample
		{
			public string A { get; set; }
			public int B { get; set; }
		}
	}
}
