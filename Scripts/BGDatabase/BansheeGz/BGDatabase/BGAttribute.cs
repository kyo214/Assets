using System;

namespace BansheeGz.BGDatabase;

public class BGAttribute : Attribute
{
	public string Name { get; set; }

	public static string GetName(Type fieldType)
	{
		BGAttribute attribute = BGUtil.GetAttribute<BGAttribute>(fieldType, inherit: true);
		if (attribute == null)
		{
			return fieldType.FullName;
		}
		return attribute.Name;
	}
}
