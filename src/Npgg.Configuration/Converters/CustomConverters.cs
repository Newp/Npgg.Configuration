
//using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace Npgg.Configuration
{

	public abstract class CustomConverter<T> : StringConverter
	{

		public abstract T Convert(string value);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			T result = this.Convert(value.ToString());//type casting è¹‚ë???tostring ??????¢Š?¤æ€???‰ìŸ¾??Žë–Ž.
			return result;
		}

		public static void RegistConverter<CONVERTER>() where CONVERTER : CustomConverter<T>
		{
			var attr = new TypeConverterAttribute(typeof(CONVERTER));
			TypeDescriptor.AddAttributes(typeof(T), attr);
		}
	}

	//public class Vector3Converter : CustomConverter<Vector3>
	//{

	//    public override Vector3 Convert(string value)
	//    {

	//        if(string.IsNullOrWhiteSpace(value))
	//        {
	//            return Vector3.zero;
	//        }

	//        if (value.StartsWith("(") && value.EndsWith(")"))
	//        {
	//            value = value.Substring(1, value.Length - 2);
	//        }

	//        value = value.Replace(" ", string.Empty);

	//        // split the items
	//        string[] sArray = value.Split(',');

	//        // store as a Vector3
	//        Vector3 result = new Vector3(
	//            float.Parse(sArray[0]),
	//            float.Parse(sArray[1]),
	//            float.Parse(sArray[2]));

	//        return result;
	//    }
	//}

}
