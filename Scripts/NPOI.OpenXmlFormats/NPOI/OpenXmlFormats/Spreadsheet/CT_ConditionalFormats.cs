using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ConditionalFormats
{
	private List<CT_ConditionalFormat> conditionalFormatField;

	private uint countField;

	[XmlElement("conditionalFormat", Order = 0)]
	public List<CT_ConditionalFormat> conditionalFormat
	{
		get
		{
			return conditionalFormatField;
		}
		set
		{
			conditionalFormatField = value;
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

	public static CT_ConditionalFormats Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ConditionalFormats cT_ConditionalFormats = new CT_ConditionalFormats();
		if (node.Attributes["count"] != null)
		{
			cT_ConditionalFormats.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_ConditionalFormats.conditionalFormat = new List<CT_ConditionalFormat>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "conditionalFormat")
			{
				cT_ConditionalFormats.conditionalFormat.Add(CT_ConditionalFormat.Parse(childNode, namespaceManager));
			}
		}
		return cT_ConditionalFormats;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (conditionalFormat != null)
		{
			foreach (CT_ConditionalFormat item in conditionalFormat)
			{
				item.Write(sw, "conditionalFormat");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ConditionalFormats()
	{
		conditionalFormatField = new List<CT_ConditionalFormat>();
		countField = 0u;
	}
}
