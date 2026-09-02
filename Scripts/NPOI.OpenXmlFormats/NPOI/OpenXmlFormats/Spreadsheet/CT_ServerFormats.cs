using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ServerFormats
{
	private List<CT_ServerFormat> serverFormatField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("serverFormat", Order = 0)]
	public List<CT_ServerFormat> serverFormat
	{
		get
		{
			return serverFormatField;
		}
		set
		{
			serverFormatField = value;
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

	public static CT_ServerFormats Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ServerFormats cT_ServerFormats = new CT_ServerFormats();
		if (node.Attributes["count"] != null)
		{
			cT_ServerFormats.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_ServerFormats.serverFormat = new List<CT_ServerFormat>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "serverFormat")
			{
				cT_ServerFormats.serverFormat.Add(CT_ServerFormat.Parse(childNode, namespaceManager));
			}
		}
		return cT_ServerFormats;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (serverFormat != null)
		{
			foreach (CT_ServerFormat item in serverFormat)
			{
				item.Write(sw, "serverFormat");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ServerFormats()
	{
		serverFormatField = new List<CT_ServerFormat>();
	}
}
