using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_RowFields
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

	public static CT_RowFields Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RowFields cT_RowFields = new CT_RowFields();
		if (node.Attributes["count"] != null)
		{
			cT_RowFields.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_RowFields.field = new List<CT_Field>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "field")
			{
				cT_RowFields.field.Add(CT_Field.Parse(childNode, namespaceManager));
			}
		}
		return cT_RowFields;
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

	public CT_RowFields()
	{
		fieldField = new List<CT_Field>();
		countField = 0u;
	}

	public CT_Field AddNewField()
	{
		CT_Field cT_Field = new CT_Field();
		fieldField.Add(cT_Field);
		return cT_Field;
	}

	public uint SizeOfFieldArray()
	{
		return (uint)fieldField.Count;
	}

	public List<CT_Field> GetFieldArray()
	{
		return fieldField;
	}

	public CT_Field GetFieldArray(int p)
	{
		return fieldField[p];
	}
}
