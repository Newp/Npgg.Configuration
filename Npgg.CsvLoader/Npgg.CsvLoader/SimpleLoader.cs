using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Npgg
{

	public class CsvLoader
    {
        const string _splitPattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        Regex regex = new Regex(_splitPattern);

		public string[] Split(string rowString)
		{
			var row = regex.Split(rowString);
			return row.Select(cell => cell.Trim(' ', '"')).ToArray();

		}
        

        public List<T> Load<T>(string tableString) where T : new()
        {
            var type = typeof(T);
            var result = new List<T>();
            using( StringReader reader = new StringReader(tableString))
            {
                var columnString = reader.ReadLine();
                var columns = new List<string>( this.Split(columnString));

				var members = type.GetMembers()
					.Where(propertyInfo => propertyInfo.MemberType == MemberTypes.Field || propertyInfo.MemberType == MemberTypes.Property)
					.ToDictionary(mem => mem.GetCustomAttribute<CsvColumnAttribute>()?.ColumnName ?? mem.Name);

				var binders = new List<BindInfo>();
                for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var columnName = columns[columnIndex];

                    if (columnName.StartsWith("#")
                        || columnName.StartsWith("//")
                        || members.TryGetValue(columnName, out var memberInfo) == false)
                    {
                        continue;
                    }

                    binders.Add(new BindInfo(columnName, columnIndex, memberInfo));
                }

                // max length check passed
                
                while(true)
                {
                    var rowString = reader.ReadLine();

                    if (rowString == null) break;

                    if (rowString.StartsWith("#") || rowString.StartsWith("//")) continue; //??낆뒩??뗫빝?


					var row = this.Split(rowString);
					T item = new T();

                    foreach (var binder in binders)
                    {
                        var cellValue = row[binder.RawIndex];
                        var convertedCellValue = binder.Converter.ConvertFromString(cellValue);

                        binder.Assigner.SetValue(item, convertedCellValue);
                    }

                    result.Add(item);
                }

            }

            return result;

        }
        
        
    }
}
