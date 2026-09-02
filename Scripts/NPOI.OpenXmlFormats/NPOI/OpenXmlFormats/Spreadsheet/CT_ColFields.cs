using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ColFields
{
	private List<CT_Field> fieldField;

	private uint countField;

	[XmlElement("field", Order = 0)]
	public List<CT_Field> field
	{
		get
		{
			return fieldField;
		}
		set
		{
			fieldField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
		}
	}

	public static CT_ColFields Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ColFields cT_ColFields = new CT_ColFields();
		if (node.Attributes["count"] != null)
		{
			cT_ColFields.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_ColFields.field = new List<CT_Field>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "field")
			{
				cT_ColFields.field.Add(CT_Field.Parse(childNode, namespaceManager));
			}
		}
		return cT_ColFields;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (field != null)
		{
			foreach (CT_Field item in field)
			{
				item.Write(sw, "field");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ColFields()
	{
		fieldField = new List<CT_Field>();
		countField = 0u;
	}

	public uint SizeOfFieldArray()
	{
		if (fieldField == null)
		{
			fieldField = new List<CT_Field>();
		}
		return (uint)fieldField.Count;
	}

	public CT_Field AddNewField()
	{
		if (fieldField == null)
		{
			fieldField = new List<CT_Field>();
		}
		CT_Field cT_Field = new CT_Field();
		fieldField.Add(cT_Field);
		return cT_Field;
	}
}
