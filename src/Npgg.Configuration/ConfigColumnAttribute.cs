using System;

namespace Npgg.Configuration
{
	[AttributeUsage(AttributeTargets.Field| AttributeTargets.Property, AllowMultiple =false)]
	public class ConfigColumnAttribute : Attribute
    {
        public ConfigColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; }
    }
}
