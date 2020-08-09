using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Npgg.CsvLoader
{




    public abstract class CsvLoader
    {

        const string _splitPattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

        protected void AllRowBind<V>(string customName, Action<V> OnRowBinded)
        {
            string tableString = this.LoadString<V>(customName);

            Type ValueType = typeof(V);
            var createRow = Expression.Lambda<Func<V>>(
                Expression.Convert(Expression.New(ValueType), ValueType))
                .Compile();

            ConfigRowBinder binder = this.LoadBinder(ValueType, tableString);
            Regex reg = new Regex(_splitPattern);
            foreach (string rowString in this.ReadTableRow(tableString))
            {
                const string _remark = "//";
                if (rowString.StartsWith(_remark))
                    continue;

                const string _empty = ",";
                if (rowString.StartsWith(_empty))
                    continue;

                string[] values = reg.Split(rowString);

                V value = createRow();

                binder.Bind(value, values);
                OnRowBinded(value);
            }
        }

        public List<V> Load<V>(string customName = null)
        {
            List<V> result = new List<V>();

            this.AllRowBind<V>(customName, (value) =>
            {
                result.Add(value);
            });

            return result;
        }

        public Dictionary<K, V> Load<K, V>(Func<V, K> keySelector, string customName = null)
        {
            return this.Load<K, V, V>(keySelector, customName);
        }

        public Dictionary<K, V> Load<K, V, SOURCE_V>(Func<V, K> keySelector, string customName = null)
            where SOURCE_V : V
        {
            Dictionary<K, V> result = new Dictionary<K, V>();

            this.AllRowBind<SOURCE_V>(customName, (value) =>
            {
                V detail = (V)value;
                K key = keySelector(detail);
                result.Add(key, detail);
            });

            return result;
        }
        

        protected virtual ConfigRowBinder LoadBinder(Type ValueType, string TableString)
        {
            ConfigRowBinder MemberBinders = new ConfigRowBinder();

            int binderCount = 0;
            MemberBinders.Clear();

            string columnString = null;
            using (StringReader reader = new StringReader(TableString))
            {

                columnString = reader.ReadLine();

                if(string.IsNullOrEmpty(columnString))
                {
                    
                }

                string[] columTextList = Regex.Split(columnString, _splitPattern);
                //string[] columTextList = columnString.Split('\t');
                for (int i = 0, t = columTextList.Length; i < t; i++)
                {
                    string columnText = columTextList[i];
                    if (columnText.StartsWith("#"))
                    {
                        MemberBinders.Add(null);
                        continue;
                    }

                    string memberName = columnText;
                    //string optionQuery = null;
                    if (columnText.Contains("?"))
                    {
                        memberName = columnText.Substring(0, columnText.IndexOf("?"));
                        //optionQuery = columnText.Substring(columnText.IndexOf("?"));
                    }


                    PropertyInfo pi = null;
                    FieldInfo fi;

                    ConfigValueBinder binder = null;

                    if ((pi = ValueType.GetProperty(memberName)) != null
                        || (pi = ValueType.GetProperty(memberName, BindingFlags.NonPublic | BindingFlags.Instance)) != null)
                    {
                        binder = new ConfigValueBinder(ValueType, memberName, pi.PropertyType, pi);
                    }
                    else if ((fi = ValueType.GetField(memberName)) != null
                        || (fi = ValueType.GetField(memberName, BindingFlags.NonPublic | BindingFlags.Instance)) != null)
                    {
                        binder = new ConfigValueBinder(ValueType, memberName, fi.FieldType, fi);
                    }
                    else
                    {
                        MemberBinders.Add(null);
                        continue;
                    }

                    MemberBinders.Add(binder);
                    binderCount++;
                }
            }
            MemberBinders.ReadyBinders();

            return MemberBinders;
        }

        protected IEnumerable<string> ReadTableRow(string TableString)
        {
            string rowString = null;

            int lineNumber = 0;
            int lineThrough = 1;
            int loaded = 0;

            using (StringReader reader = new StringReader(TableString))
            {
                while (true)
                {
                    if ((rowString = reader.ReadLine()) == null)
                        break;

                    if (lineNumber++ < lineThrough)
                    {
                        lineNumber++;
                        continue;
                    }

                    loaded++;
                    yield return rowString;
                }
            }

            yield break;
        }


        public abstract string LoadString<T>(string customName);



        public void RegistConverter<VALUE_TYPE, CONVERTER>() where CONVERTER : CustomConverter<VALUE_TYPE>
        {
            var attr = new TypeConverterAttribute(typeof(CONVERTER));
            TypeDescriptor.AddAttributes(typeof(VALUE_TYPE), attr);
        }
    }
}
