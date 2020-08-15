using System;

namespace Npgg.Configuration
{
	public class RequiredColumnNotFoundException : Exception
	{
		public RequiredColumnNotFoundException(string columnName) : base($"not found column=>{columnName}")
		{
			ColumnName = columnName;
		}

		public string ColumnName { get; }
	}
}
