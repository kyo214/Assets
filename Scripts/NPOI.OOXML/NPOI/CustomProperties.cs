using System;
using NPOI.OpenXmlFormats;

namespace NPOI;

public class CustomProperties
{
	public static string FORMAT_ID = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";

	public CustomPropertiesDocument props;

	internal CustomProperties(CustomPropertiesDocument props)
	{
		this.props = props;
	}

	public CT_CustomProperties GetUnderlyingProperties()
	{
		return props.GetProperties();
	}

	private CT_Property Add(string name)
	{
		if (Contains(name))
		{
			throw new ArgumentException("A property with this name already exists in the custom properties");
		}
		CT_Property cT_Property = props.GetProperties().AddNewProperty();
		int pid = NextPid();
		cT_Property.pid = pid;
		cT_Property.fmtid = FORMAT_ID;
		cT_Property.name = name;
		return cT_Property;
	}

	public void AddProperty(string name, string value)
	{
		CT_Property cT_Property = Add(name);
		cT_Property.ItemElementName = ItemChoiceType.lpwstr;
		cT_Property.Item = value;
	}

	public void AddProperty(string name, double value)
	{
		CT_Property cT_Property = Add(name);
		cT_Property.ItemElementName = ItemChoiceType.r8;
		cT_Property.Item = value;
	}

	public void AddProperty(string name, int value)
	{
		CT_Property cT_Property = Add(name);
		cT_Property.ItemElementName = ItemChoiceType.i4;
		cT_Property.Item = value;
	}

	public void AddProperty(string name, bool value)
	{
		CT_Property cT_Property = Add(name);
		cT_Property.ItemElementName = ItemChoiceType.@bool;
		cT_Property.Item = value;
	}

	protected int NextPid()
	{
		int num = 1;
		foreach (CT_Property property in props.GetProperties().GetPropertyList())
		{
			if (property.pid > num)
			{
				num = property.pid;
			}
		}
		return num + 1;
	}

	public bool Contains(string name)
	{
		foreach (CT_Property property in props.GetProperties().GetPropertyList())
		{
			if (property.name.Equals(name))
			{
				return true;
			}
		}
		return false;
	}

	public CT_Property GetProperty(string name)
	{
		foreach (CT_Property property in props.GetProperties().GetPropertyList())
		{
			if (property.name.Equals(name))
			{
				return property;
			}
		}
		return null;
	}
}
