using Npgg.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Npgg.Reflection;

namespace Npgg.Configuration
{
   
    public class BindInfo
    {
        public string ColumnName { get; set; }
        public int RawIndex { get; set; }
        public MemberAccessor Assigner { get; set; }
        public TypeConverter Converter { get; set; }

        public BindInfo(string columnName, int rawIndex, MemberInfo memberInfo)
        {
            this.ColumnName = columnName;
            this.RawIndex = rawIndex;
            this.Assigner = new MemberAccessor(memberInfo);

            if (Assigner.ValueType.IsGenericType)
            {
                Type genericType = Assigner.ValueType.GetGenericTypeDefinition();
                Type[] argTypes = Assigner.ValueType.GetGenericArguments();

                if (genericType == typeof(List<>))
                {
                    this.Converter = new ListCustomConverter(genericType.MakeGenericType(argTypes[0]), argTypes[0]);
                }
				else if (genericType == typeof(Nullable<>))
				{
					this.Converter = TypeDescriptor.GetConverter(this.Assigner.ValueType);
				}
            }
            else if (Assigner.ValueType.IsArray)
            {
                this.Converter = new ArrayCustomConverter(Assigner.ValueType.GetElementType());
            }
            else
            {
                this.Converter = TypeDescriptor.GetConverter(this.Assigner.ValueType);
			}


			if (this.Converter == null)
			{
				throw new Exception($"not found converter for {columnName} - {this.Assigner.ValueType.Name}");
			}
		}
    }
    
}
