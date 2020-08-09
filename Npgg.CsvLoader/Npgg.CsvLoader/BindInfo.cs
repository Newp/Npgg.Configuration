using System.ComponentModel;
using System.Reflection;

namespace Npgg.CsvLoader
{
    public partial class SimpleLoader
    {
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
