using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Npgg.Configuration
{

	public class CsvLoader : ConfigurationLoader
	{
		static CsvLoader _shared = null;
		public static CsvLoader Shared =>_shared ?? (_shared = new CsvLoader());
		public const string SplitPatten = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
		public CsvLoader() : base(SplitPatten)
		{

		}
	}
	public class TsvLoader : ConfigurationLoader
	{
		static TsvLoader _shared = null;
		public static TsvLoader Shared => _shared ?? (_shared = new TsvLoader());
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


		public virtual List<T> Load<T>(string tableString) where T : new()
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

					if (rowString.StartsWith("#") || rowString.StartsWith("//")) continue; //?????⑹름??????뭽?


					var rowNumber = this.Split(rowString);
					T item = new T();

					foreach (var binder in binders)
					{

						string cellValue = rowNumber.Length > binder.RawIndex ? rowNumber[binder.RawIndex] : string.Empty ;

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


		public string ToCsvString<T>(IEnumerable<T> values) => ObjectToString<T>(values, ',');
		public string ToTsvString<T>(IEnumerable<T> values) => ObjectToString<T>(values, '\t');

		string ObjectToString<T>(IEnumerable<T> values, char spliter)
		{
			var accessors = Reflection.MemberAccessor.GetAccessors<T>();

			StringBuilder builder = new StringBuilder();

			foreach (var accessor in accessors)
			{
				builder.Append($"{accessor.Key}");
				builder.Append(spliter);
			}

			builder.Length--;
			builder.Append('\n');

			foreach (var item in values)
			{
				foreach (var accessor in accessors)
				{
					var value = accessor.Value.GetValue(item);
					builder.Append($"\"{value}\"");
					builder.Append(spliter);
				}

				builder.Length--;
				builder.Append('\n');
			}

			builder.Length--;
			return builder.ToString();
		}
	}
}
