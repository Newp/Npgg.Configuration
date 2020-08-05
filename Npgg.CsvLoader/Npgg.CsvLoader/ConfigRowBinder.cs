using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace Npgg.CsvLoader
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

        //public T GetValue<T>(string ColumnName, string[] values)
        //{
        //    ConfigValueBinder binder = null;
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        binder = this[i];

        //        if (binder != null && binder.ColumnName == ColumnName)
        //        {
        //            return (T)binder.GetValue(values[i]);
        //        }
        //    }

        //    throw new KeyNotFoundException(string.Format("index : {0}, values : {1}", ColumnName, string.Join(",", values)));
        //}
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
