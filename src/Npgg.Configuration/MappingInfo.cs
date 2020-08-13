using System.Reflection;

namespace Npgg.Configuration
{
	class MappingInfo
	{
		public bool Required { get; }
		public string ColumnName { get; }
		public MemberInfo MemberInfo { get; }

		public MappingInfo(MemberInfo memberInfo)
		{
			this.MemberInfo = memberInfo;
			var attr = memberInfo.GetCustomAttribute<ConfigColumnAttribute>();

			if (attr == null)
			{
				this.ColumnName = this.MemberInfo.Name;
				this.Required = false;
			}
			else
			{
				this.ColumnName = attr.ColumnName ?? this.MemberInfo.Name;
				this.Required = attr.Required;
			}
		}

	}
}
