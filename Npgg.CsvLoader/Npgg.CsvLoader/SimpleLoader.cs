using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Npgg.CsvLoader
{
    public class SimpleLoader
    {
        const string _splitPattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        Regex regex = new Regex(_splitPattern);
        protected void SplitRow(string rowString) => regex.Split(rowString);


        public async Task<List<T>> Load<T>(string tableString) where T : new()
        {
            var type = typeof(T);
            var result = new List<T>();
            using( StringReader reader = new StringReader(tableString))
            {
                var columnString = await reader.ReadLineAsync();
                var columns = new List<string>(regex.Split(columnString));

                var members = type.GetMembers()
                    .Where(pi => pi.MemberType == MemberTypes.Field || pi.MemberType == MemberTypes.Property)
                    .Where(pi => columns.Contains( pi.Name))
                    .ToDictionary(member => member.Name);


                var binders = new List<BindInfo>();
                for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var columnName = columns[columnIndex];

                    if (members.TryGetValue(columnName, out var memberInfo) == false)
                    {
                        continue;
                    }

                    binders.Add(new BindInfo(columnName, columnIndex, memberInfo));
                }

                // max length check passed
                
                while(true)
                {
                    var line = await reader.ReadLineAsync();

                    if (line == null) break;

                    var row = regex.Split(line);
                    T item = new T();

                    foreach (var binder in binders)
                    {
                        var rawValue = row[binder.RawIndex];
                        var converted = binder.Converter.ConvertFromString(rawValue);

                        binder.Assigner.SetValue(item, converted);
                    }

                    result.Add(item);
                }

            }

            return result;

        }

        public class BindInfo
        {
            public string ColumnName { get; set; }
            public int RawIndex { get; set; }
            public MemberAssigner Assigner { get; set; }
            public TypeConverter Converter { get; set; }

            public BindInfo(string columnName, int rawIndex, MemberInfo memberInfo)
            {
                this.ColumnName = columnName;
                this.RawIndex = rawIndex;
                this.Assigner = new MemberAssigner(memberInfo);
                this.Converter = TypeDescriptor.GetConverter(this.Assigner.ValueType);
            }
        }
        
        
    }
}
