using System;

namespace Npgg.Configuration
{
	public class CsvColumnAttribute : Attribute
    {
        public CsvColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; }
    }
}
