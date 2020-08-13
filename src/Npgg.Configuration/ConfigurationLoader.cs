using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Npgg.Configuration
{

	public class CsvLoader : ConfigurationLoader
	{
		public const string SplitPatten = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
		public CsvLoader() : base(SplitPatten)
		{

		}
	}
	public class TsvLoader : ConfigurationLoader
	{

		public const string SplitPatten = "\t(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

		public TsvLoader() : base(SplitPatten)
		{

		}
	}

	

	public abstract class ConfigurationLoader
	{
		readonly Regex regex;

		internal ConfigurationLoader(string splitPattern)
		{
			this.regex = new Regex(splitPattern);
		}

		public string[] Split(string rowString)
		{
			var row = regex.Split(rowString);
			return row.Select(cell => cell.Trim(' ', '"')).ToArray();
		}

		

		public List<T> Load<T>(string tableString) where T : new()
		{
			var type = typeof(T);
			var result = new List<T>();
			using (StringReader reader = new StringReader(tableString))
			{
				var columnString = reader.ReadLine();
				var columns = new List<string>(this.Split(columnString));

				var members = type.GetMembers()
					.Where(propertyInfo => propertyInfo.MemberType == MemberTypes.Field || propertyInfo.MemberType == MemberTypes.Property)
					.Select(memberInfo => new MappingInfo(memberInfo))
					.ToDictionary(mappingInfo => mappingInfo.ColumnName);

				var binders = new List<BindInfo>();
				for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
				{
					var columnName = columns[columnIndex];

					if (columnName.StartsWith("#")
						|| columnName.StartsWith("//")
						|| members.TryGetValue(columnName, out var mappingInfo) == false)
					{
						continue;
					}

					binders.Add(new BindInfo(columnName, columnIndex, mappingInfo.MemberInfo));
				}

				foreach(var member in members.Values)
				{
					if (member.Required == true
						&& binders.Where(binder => binder.ColumnName == member.ColumnName).Count() == 0)
						throw new RequiredColumnNotFoundException(member.ColumnName);
				}

				// max length check passed

				int lineNumber = 0;
				while (true)
				{
					var rowString = reader.ReadLine();
					lineNumber++;

					if (rowString == null) break;

					if (rowString.StartsWith("#") || rowString.StartsWith("//")) continue; //????용츧????ロ뒌?


					var rowNumber = this.Split(rowString);
					T item = new T();

					foreach (var binder in binders)
					{
						var cellValue = rowNumber[binder.RawIndex];

						try
						{
							var convertedCellValue = binder.Converter.ConvertFromString(cellValue);
							binder.Assigner.SetValue(item, convertedCellValue);
						}
						catch (Exception ex)
						{
							throw new ConvertException(binder.ColumnName, cellValue, lineNumber, ex);
						}

					}

					result.Add(item);
				}

			}

			return result;

		}


	}

	public class ConvertException : Exception
	{
		public ConvertException(string columnName , string textValue, int lineNumber, Exception exception) : 
			base($"convert error => column:{columnName}, text:{textValue}, line:{lineNumber}", exception)
		{
			ColumnName = columnName;
			TextValue = textValue;
			LineNumber = lineNumber;
		}

		public string ColumnName { get; }
		public string TextValue { get; }
		public int LineNumber { get; }
	}

	public class RequiredColumnNotFoundException : Exception
	{
		public RequiredColumnNotFoundException(string columnName)
		{
			ColumnName = columnName;
		}

		public string ColumnName { get; }
	}
}
