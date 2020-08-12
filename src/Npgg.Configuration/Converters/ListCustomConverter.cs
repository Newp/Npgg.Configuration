using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace Npgg.Configuration
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
}
