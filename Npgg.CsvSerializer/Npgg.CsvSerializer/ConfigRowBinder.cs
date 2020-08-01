using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace Npgg.Configuration
{
    public class ConfigRowBinder : List<ConfigValueBinder>
    {
        public void ReadyBinders()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if(this[i] == null)
                    continue;

                _validatedBinders.Add( new KeyValuePair<int, ConfigValueBinder>( i, this[i]));
            }
        }

        List<KeyValuePair<int, ConfigValueBinder>> _validatedBinders = new List<KeyValuePair<int, ConfigValueBinder>>();


        public void Bind(object target, string[] values)
        {
            KeyValuePair < int, ConfigValueBinder > binder;

            for (int i = 0; i < _validatedBinders.Count; i++)
            {
                binder = _validatedBinders[i];

                try
                {
                    binder.Value.Bind(target, values[binder.Key].Trim('\"'));
                }
                catch (Exception)
                {
                    throw new ConfigValueBindingException(_validatedBinders[i].Value, values[binder.Key]);
                }
                
            }
        }

        public T GetValue<T>(string ColumnName, string[] values)
        {
            ConfigValueBinder binder = null;
            for (int i = 0; i < this.Count; i++)
            {
                binder = this[i];

                if (binder != null && binder.ColumnName == ColumnName)
                {
                    return (T)binder.GetValue(values[i]);
                }
            }

            throw new KeyNotFoundException(string.Format("index : {0}, values : {1}", ColumnName, string.Join(",", values)));
        }
    }

    public class ConfigValueBinder
    {

        MemberSetter setter = null; 
        

        public Type TableType { get; private set; }
        public string ColumnName { get; private set; }
        public Type ValueType { get; private set; }
        public Type CollectionType { get; private set; }
        private List<string> _enumList = null;
        public IEnumerable<string> EnumList { get { return this._enumList; } }

        private Func<string, object> ValueParser { get; set; }
        private Action<string, object> ValueBinder { get; set; }
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

        private bool EnumTryParse(string value, out object result)
        {
            if (this._enumList.Contains(value))
            {
                result = Enum.Parse(ValueType, value);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }


    public abstract class CustomConverter<T> : StringConverter
    {

        public abstract T Convert(string value);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            T result = this.Convert(value.ToString());//type casting 보다 tostring 이 더 빠르고 안전하다.
            return result;
        }
    }



    public interface MemberSetter
    {
        void Set(object target, object value);

    }

    public class PropertySetter : MemberSetter
    {
        PropertyInfo pinfo = null;
        public PropertySetter(PropertyInfo info)
        {
            this.pinfo = info;
        }

        public void Set(object target, object value)
        {
            this.pinfo.SetValue(target, value);
        }
    }

    public class FieldSetter : MemberSetter
    {
        FieldInfo finfo = null;
        public FieldSetter(FieldInfo info)
        {
            this.finfo = info;
        }

        public void Set(object target, object value)
        {
            this.finfo.SetValue(target, value);
        }
    }

    public class ConfigValueBindingException : Exception
    {
        public Type TableType { get; private set; }
        public string ColumnName { get; private set; }
        public Type ValueType { get; private set; }
        public string Value { get; private set; }

        private ConfigValueBinder _binder = null;

        public ConfigValueBindingException(ConfigValueBinder Binder, string Value) : base(
            string.Format("can not parsing value for binding property => table:{0}, column:{1}, type:{2}, value:{3}"
                , Binder.TableType.FullName, Binder.ColumnName, Binder.ValueType.FullName, Value))
        {
            this._binder = Binder;
            this.TableType = _binder.TableType;
            this.ColumnName = _binder.ColumnName;
            this.ValueType = _binder.ValueType;
            this.Value = Value;
        }

    }
}
