using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace Npgg.CsvLoader
{
    public class ConfigValueBinder
    {

        MemberSetter setter = null; 
        

        public Type TableType { get; private set; }
        public string ColumnName { get; private set; }
        public Type ValueType { get; private set; }
        public Type CollectionType { get; private set; }
        

        private Func<string, object> ValueParser { get; set; }
        
        public Action<object, string> Bind { get; private set; }
        

        public ConfigValueBinder(Type TableType, string ColumnName, Type MemberType, MemberInfo MemberInfo)
        {

            if (MemberInfo is PropertyInfo)
            {
                this.setter = new PropertySetter((PropertyInfo)MemberInfo);
            }
            else if (MemberInfo is FieldInfo)
            {
                this.setter = new FieldSetter((FieldInfo)MemberInfo);
            }
            else
            {
                throw new Exception("ValueBinder MemberType Error");
            }
            
            this.TableType = TableType;
            this.ColumnName = ColumnName;

            if (MemberType.IsGenericType)
            {
                Type listType = MemberType.GetGenericTypeDefinition();
                Type[] argTypes = MemberType.GetGenericArguments();
                this.ValueType = argTypes[0];
                if (listType == typeof(List<>))
                {
                    this.Bind = this.BindList;
                    this.CollectionType = listType.MakeGenericType(this.ValueType);
                }
                else
                {
                    this.Bind = this.BindOne;
                    //throw new Exception(string.Format("can not parse {0} type", listType.FullName));
                }
            }
            else
            {
                this.ValueType = MemberType;
                this.Bind = this.BindOne;
            }

            var converter = TypeDescriptor.GetConverter(ValueType);
            if (converter == null)
            {
                this.GetCustomParser(ValueType);
            }
            else
            {
                this.ValueParser = converter.ConvertFromString;
            }
        }

        private void GetCustomParser(Type type)
        {
            throw new NotImplementedException(string.Format("can not parse {0} type", type.FullName));   
        }
        

        public static char[] ListSeparator = new char[] { ' ', ',' };
        private void BindList(object obj, string listText)
        {
            IList parsedList = (IList)Activator.CreateInstance(this.CollectionType);

            foreach (string str in listText.Split(ListSeparator))
            {
                object parsedValue = this.ValueParser(str);
                parsedList.Add(parsedValue);
            }

            this.SetValue(obj, parsedList);
        }

        private void BindOne(object obj, string value)
        {
            object parsedValue = this.ValueParser(value);

            this.SetValue(obj, parsedValue);
        }

        public object GetValue(string value)
        {
            object parsedValue = this.ValueParser(value);
            return parsedValue;
        }

        public void SetValue(object obj, object value)
        {
            this.setter.Set(obj, value);
        }

    }
}
