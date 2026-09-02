using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PageFields
{
	private List<CT_PageField> pageFieldField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("pageField", Order = 0)]
	public List<CT_PageField> pageField
	{
		get
		{
			return pageFieldField;
		}
		set
		{
			pageFieldField = value;
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

	public static CT_PageFields Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PageFields cT_PageFields = new CT_PageFields();
		if (node.Attributes["count"] != null)
		{
			cT_PageFields.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PageFields.pageField = new List<CT_PageField>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pageField")
			{
				cT_PageFields.pageField.Add(CT_PageField.Parse(childNode, namespaceManager));
			}
		}
		return cT_PageFields;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (pageField != null)
		{
			foreach (CT_PageField item in pageField)
			{
				item.Write(sw, "pageField");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PageFields()
	{
		pageFieldField = new List<CT_PageField>();
	}

	public CT_PageField AddNewPageField()
	{
		if (pageFieldField == null)
		{
			pageFieldField = new List<CT_PageField>();
		}
		CT_PageField cT_PageField = new CT_PageField();
		pageFieldField.Add(cT_PageField);
		return cT_PageField;
	}

	public uint SizeOfPageFieldArray()
	{
		if (pageFieldField == null)
		{
			pageFieldField = new List<CT_PageField>();
		}
		return (uint)pageFieldField.Count;
	}
}
