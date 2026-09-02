using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PCDSCPage
{
	private List<CT_PageItem> pageItemField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("pageItem", Order = 0)]
	public List<CT_PageItem> pageItem
	{
		get
		{
			return pageItemField;
		}
		set
		{
			pageItemField = value;
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

	public static CT_PCDSCPage Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PCDSCPage cT_PCDSCPage = new CT_PCDSCPage();
		if (node.Attributes["count"] != null)
		{
			cT_PCDSCPage.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PCDSCPage.pageItem = new List<CT_PageItem>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pageItem")
			{
				cT_PCDSCPage.pageItem.Add(CT_PageItem.Parse(childNode, namespaceManager));
			}
		}
		return cT_PCDSCPage;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (pageItem != null)
		{
			foreach (CT_PageItem item in pageItem)
			{
				item.Write(sw, "pageItem");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PCDSCPage()
	{
		pageItemField = new List<CT_PageItem>();
	}
}
