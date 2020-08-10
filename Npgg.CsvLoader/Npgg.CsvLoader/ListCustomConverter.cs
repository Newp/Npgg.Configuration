using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace Npgg.CsvLoader
{
    public class ListCustomConverter : StringConverter
    {
        readonly TypeConverter converter;
        readonly Type collectionType;
        public ListCustomConverter(Type collectionType, Type itemType)
        {
            this.collectionType = collectionType;

            this.converter = TypeDescriptor.GetConverter(itemType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var splited = value.ToString().Trim('\"').Split(',');

            IList parsedList = (IList)Activator.CreateInstance(this.collectionType);

            foreach(var stringItem in splited )
            {
                parsedList.Add(this.converter.ConvertFromString(stringItem));
            }

            return parsedList;
        }
    }

    public class ArrayCustomConverter : StringConverter
    {
        readonly TypeConverter converter;
        private readonly Type itemType;

        public ArrayCustomConverter(Type itemType)
        {
            this.itemType = itemType;
            this.converter = TypeDescriptor.GetConverter(itemType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var splited = value.ToString().Trim('\"').Split(',');

            var array = Array.CreateInstance(this.itemType, splited.Length);

            for(int i =0;i<splited.Length;i++)
            {
                array.SetValue(this.converter.ConvertFromString(splited[i]), i);
            }

            return array;
        }
    }

    //public class CustomConverter<T> : StringConverter
    //{
    //    readonly TypeConverter converter;
    //    readonly Type collectionType;
    //    public ListCustomConverter(Type collectionType, Type itemType)
    //    {
    //        this.collectionType = collectionType;

    //        this.converter = TypeDescriptor.GetConverter(itemType);
    //    }

    //    public

    //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    //    {
    //        var splited = value.ToString().Trim('\"').Split(',');

    //        IList parsedList = (IList)Activator.CreateInstance(this.collectionType);

    //        foreach (var stringItem in splited)
    //        {
    //            parsedList.Add(this.converter.ConvertFromString(stringItem));
    //        }

    //        return parsedList;
    //    }
    //}
}
