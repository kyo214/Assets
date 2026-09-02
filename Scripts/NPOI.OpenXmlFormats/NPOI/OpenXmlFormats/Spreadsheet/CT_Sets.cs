using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Sets
{
	private List<CT_Set> setField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("set", Order = 0)]
	public List<CT_Set> set
	{
		get
		{
			return setField;
		}
		set
		{
			setField = value;
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

	public static CT_Sets Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Sets cT_Sets = new CT_Sets();
		if (node.Attributes["count"] != null)
		{
			cT_Sets.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_Sets.set = new List<CT_Set>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "set")
			{
				cT_Sets.set.Add(CT_Set.Parse(childNode, namespaceManager));
			}
		}
		return cT_Sets;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (set != null)
		{
			foreach (CT_Set item in set)
			{
				item.Write(sw, "set");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Sets()
	{
		setField = new List<CT_Set>();
	}
}
