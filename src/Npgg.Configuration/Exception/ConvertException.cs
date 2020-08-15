using System;

namespace Npgg.Configuration
{
	public class ConvertException : Exception
	{
		public ConvertException(string columnName , string textValue, int lineNumber, Exception exception) : 
			base($"convert error => column:{columnName}, text:{textValue}, line:{lineNumber}", exception)
		{
			ColumnName = columnName;
			TextValue = textValue;
			LineNumber = lineNumber;
		}

		public string ColumnName { get; }
		public string TextValue { get; }
		public int LineNumber { get; }
	}
}
