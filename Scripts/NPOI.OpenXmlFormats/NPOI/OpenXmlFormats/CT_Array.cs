using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
public class CT_Array
{
	private object[] itemsField;

	private ItemsChoiceType[] itemsElementNameField;

	private int lBoundsField;

	private int uBoundsField;

	private ST_ArrayBaseType baseTypeField;

	[XmlElement("bool", typeof(bool))]
	[XmlElement("bstr", typeof(string))]
	[XmlElement("cy", typeof(string))]
	[XmlElement("date", typeof(DateTime))]
	[XmlElement("decimal", typeof(decimal))]
	[XmlElement("error", typeof(string))]
	[XmlElement("i1", typeof(sbyte))]
	[XmlElement("i2", typeof(short))]
	[XmlElement("i4", typeof(int))]
	[XmlElement("int", typeof(int))]
	[XmlElement("r4", typeof(float))]
	[XmlElement("r8", typeof(double))]
	[XmlElement("ui1", typeof(byte))]
	[XmlElement("ui2", typeof(ushort))]
	[XmlElement("ui4", typeof(uint))]
	[XmlElement("uint", typeof(uint))]
	[XmlElement("variant", typeof(CT_Variant))]
	[XmlChoiceIdentifier("ItemsElementName")]
	public object[] Items
	{
		get
		{
			return itemsField;
		}
		set
		{
			itemsField = value;
		}
	}

	[XmlIgnore]
	public ItemsChoiceType[] ItemsElementName
	{
		get
		{
			return itemsElementNameField;
		}
		set
		{
			itemsElementNameField = value;
		}
	}

	[XmlAttribute]
	public int lBounds
	{
		get
		{
			return lBoundsField;
		}
		set
		{
			lBoundsField = value;
		}
	}

	[XmlAttribute]
	public int uBounds
	{
		get
		{
			return uBoundsField;
		}
		set
		{
			uBoundsField = value;
		}
	}

	[XmlAttribute]
	public ST_ArrayBaseType baseType
	{
		get
		{
			return baseTypeField;
		}
		set
		{
			baseTypeField = value;
		}
	}
}
