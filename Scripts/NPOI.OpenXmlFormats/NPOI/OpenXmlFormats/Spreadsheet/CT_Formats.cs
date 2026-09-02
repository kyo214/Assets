using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Formats
{
	private List<CT_Format> formatField;

	private uint countField;

	[XmlElement("format", Order = 0)]
	public List<CT_Format> format
	{
		get
		{
			return formatField;
		}
		set
		{
			formatField = value;
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

	public static CT_Formats Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Formats cT_Formats = new CT_Formats();
		if (node.Attributes["count"] != null)
		{
			cT_Formats.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_Formats.format = new List<CT_Format>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "format")
			{
				cT_Formats.format.Add(CT_Format.Parse(childNode, namespaceManager));
			}
		}
		return cT_Formats;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (format != null)
		{
			foreach (CT_Format item in format)
			{
				item.Write(sw, "format");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Formats()
	{
		formatField = new List<CT_Format>();
		countField = 0u;
	}
}
