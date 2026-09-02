using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_colItems
{
	private List<CT_I> iField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("i", Order = 0)]
	public List<CT_I> i
	{
		get
		{
			return iField;
		}
		set
		{
			iField = value;
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

	public static CT_colItems Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_colItems cT_colItems = new CT_colItems();
		if (node.Attributes["count"] != null)
		{
			cT_colItems.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_colItems.i = new List<CT_I>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "i")
			{
				cT_colItems.i.Add(CT_I.Parse(childNode, namespaceManager));
			}
		}
		return cT_colItems;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (i != null)
		{
			foreach (CT_I item in i)
			{
				item.Write(sw, "i");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_colItems()
	{
		iField = new List<CT_I>();
	}
}
