using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_DataFields
{
	private List<CT_DataField> dataFieldField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("dataField", Order = 0)]
	public List<CT_DataField> dataField
	{
		get
		{
			return dataFieldField;
		}
		set
		{
			dataFieldField = value;
		}
	}

	[XmlAttribute]
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

	[XmlIgnore]
	public bool countSpecified
	{
		get
		{
			return countFieldSpecified;
		}
		set
		{
			countFieldSpecified = value;
		}
	}

	public static CT_DataFields Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_DataFields cT_DataFields = new CT_DataFields();
		if (node.Attributes["count"] != null)
		{
			cT_DataFields.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_DataFields.dataField = new List<CT_DataField>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "dataField")
			{
				cT_DataFields.dataField.Add(CT_DataField.Parse(childNode, namespaceManager));
			}
		}
		return cT_DataFields;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (dataField != null)
		{
			foreach (CT_DataField item in dataField)
			{
				item.Write(sw, "dataField");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_DataFields()
	{
		dataFieldField = new List<CT_DataField>();
	}

	public CT_DataField AddNewDataField()
	{
		if (dataFieldField == null)
		{
			dataFieldField = new List<CT_DataField>();
		}
		CT_DataField cT_DataField = new CT_DataField();
		dataFieldField.Add(cT_DataField);
		return cT_DataField;
	}

	public uint SizeOfDataFieldArray()
	{
		return (uint)dataFieldField.Count;
	}
}
