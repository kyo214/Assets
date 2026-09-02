using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_MemberProperties
{
	private List<CT_MemberProperty> mpField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("mp", Order = 0)]
	public List<CT_MemberProperty> mp
	{
		get
		{
			return mpField;
		}
		set
		{
			mpField = value;
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

	public static CT_MemberProperties Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_MemberProperties cT_MemberProperties = new CT_MemberProperties();
		if (node.Attributes["count"] != null)
		{
			cT_MemberProperties.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_MemberProperties.mp = new List<CT_MemberProperty>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "mp")
			{
				cT_MemberProperties.mp.Add(CT_MemberProperty.Parse(childNode, namespaceManager));
			}
		}
		return cT_MemberProperties;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (mp != null)
		{
			foreach (CT_MemberProperty item in mp)
			{
				item.Write(sw, "mp");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_MemberProperties()
	{
		mpField = new List<CT_MemberProperty>();
	}
}
