using Npgg.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

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

			this.Converter = GetConverter(this.Assigner.ValueType);


			if (this.Converter == null)
			{
				throw new Exception($"not found converter for {columnName} - {this.Assigner.ValueType.Name}");
			}
		}

		public static TypeConverter GetConverter(Type valueType)
		{

			if (valueType.IsGenericType)
			{
				Type genericType = valueType.GetGenericTypeDefinition();
				Type[] argTypes = valueType.GetGenericArguments();

				if (genericType == typeof(List<>))
				{
					return new ListCustomConverter(genericType.MakeGenericType(argTypes[0]), argTypes[0]);
				}
				else if (genericType == typeof(Nullable<>))
				{
					return TypeDescriptor.GetConverter(valueType);
				}
			}
			else if (valueType.IsArray)
			{
				return new ArrayCustomConverter(valueType.GetElementType());
			}
			
			return TypeDescriptor.GetConverter(valueType);
		}
    }
    
}
