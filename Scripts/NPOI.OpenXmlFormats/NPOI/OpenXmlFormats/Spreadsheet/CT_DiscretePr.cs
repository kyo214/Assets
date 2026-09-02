using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_DiscretePr
{
	private List<CT_Index> xField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("x", Order = 0)]
	public List<CT_Index> x
	{
		get
		{
			return xField;
		}
		set
		{
			xField = value;
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

	public static CT_DiscretePr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_DiscretePr cT_DiscretePr = new CT_DiscretePr();
		if (node.Attributes["count"] != null)
		{
			cT_DiscretePr.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_DiscretePr.x = new List<CT_Index>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "x")
			{
				cT_DiscretePr.x.Add(CT_Index.Parse(childNode, namespaceManager));
			}
		}
		return cT_DiscretePr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (x != null)
		{
			foreach (CT_Index item in x)
			{
				item.Write(sw, "x");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_DiscretePr()
	{
		xField = new List<CT_Index>();
	}
}
