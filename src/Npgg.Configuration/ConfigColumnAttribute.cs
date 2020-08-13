using System;

namespace Npgg.Configuration
{
	[AttributeUsage(AttributeTargets.Field| AttributeTargets.Property, AllowMultiple =false)]
	public class ConfigColumnAttribute : Attribute
    {


		public ConfigColumnAttribute(string columnName): this(columnName, true)
		{

		}

		public ConfigColumnAttribute(string columnName, bool required)
		{
			ColumnName = columnName;
			this.Required = required;
		}

		public ConfigColumnAttribute()
		{
			Required = true;
		}

		public string ColumnName { get; }

		public bool Required { get; }

	}
}
