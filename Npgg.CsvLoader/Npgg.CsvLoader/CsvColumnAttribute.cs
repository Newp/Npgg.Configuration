using System;

namespace Npgg
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
