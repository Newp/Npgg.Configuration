using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Npgg
{
    public partial class CsvLoader
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

                if (Assigner.ValueType.IsGenericType)
                {
                    Type listType = Assigner.ValueType.GetGenericTypeDefinition();
                    Type[] argTypes = Assigner.ValueType.GetGenericArguments();

                    if (listType == typeof(List<>))
                    {
                        this.Converter = new ListCustomConverter(listType.MakeGenericType(argTypes[0]), argTypes[0]);
                    }
                }
                else if (Assigner.ValueType.IsArray)
                {

                    this.Converter = new ArrayCustomConverter(Assigner.ValueType.GetElementType());
                    //.MakeArrayType()
                }
                else
                {
                    this.Converter = TypeDescriptor.GetConverter(this.Assigner.ValueType);
                }
            }
        }
    }
}
