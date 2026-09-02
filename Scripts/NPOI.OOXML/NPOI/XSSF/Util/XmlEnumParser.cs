using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace NPOI.XSSF.Util;

public class XmlEnumParser<TReturn>
{
	private static Dictionary<string, TReturn> values;

	static XmlEnumParser()
	{
		Type typeFromHandle = typeof(TReturn);
		MemberInfo[] members = typeFromHandle.GetMembers(BindingFlags.Static | BindingFlags.Public);
		Enum.GetNames(typeFromHandle);
		values = new Dictionary<string, TReturn>();
		typeFromHandle.GetEnumValues();
		MemberInfo[] array = members;
		foreach (MemberInfo memberInfo in array)
		{
			object[] customAttributes = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				XmlEnumAttribute xmlEnumAttribute = (XmlEnumAttribute)customAttributes[0];
				values.Add(xmlEnumAttribute.Name, (TReturn)Enum.Parse(typeFromHandle, memberInfo.Name));
			}
		}
	}

	public static TReturn ForName(string name, TReturn defaultValue)
	{
		if (values.ContainsKey(name))
		{
			return values[name];
		}
		return defaultValue;
	}
}
