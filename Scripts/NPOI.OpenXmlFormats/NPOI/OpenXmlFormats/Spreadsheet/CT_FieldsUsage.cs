using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_FieldsUsage
{
	private List<CT_FieldUsage> fieldUsageField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("fieldUsage", Order = 0)]
	public List<CT_FieldUsage> fieldUsage
	{
		get
		{
			return fieldUsageField;
		}
		set
		{
			fieldUsageField = value;
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

	public static CT_FieldsUsage Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FieldsUsage cT_FieldsUsage = new CT_FieldsUsage();
		if (node.Attributes["count"] != null)
		{
			cT_FieldsUsage.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_FieldsUsage.fieldUsage = new List<CT_FieldUsage>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "fieldUsage")
			{
				cT_FieldsUsage.fieldUsage.Add(CT_FieldUsage.Parse(childNode, namespaceManager));
			}
		}
		return cT_FieldsUsage;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (fieldUsage != null)
		{
			foreach (CT_FieldUsage item in fieldUsage)
			{
				item.Write(sw, "fieldUsage");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_FieldsUsage()
	{
		fieldUsageField = new List<CT_FieldUsage>();
	}
}
