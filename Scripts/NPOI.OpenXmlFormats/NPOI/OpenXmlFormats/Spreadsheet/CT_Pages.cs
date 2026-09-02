using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Pages
{
	private List<CT_PCDSCPage> pageField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("page", Order = 0)]
	public List<CT_PCDSCPage> page
	{
		get
		{
			return pageField;
		}
		set
		{
			pageField = value;
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

	public static CT_Pages Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Pages cT_Pages = new CT_Pages();
		if (node.Attributes["count"] != null)
		{
			cT_Pages.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_Pages.page = new List<CT_PCDSCPage>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "page")
			{
				cT_Pages.page.Add(CT_PCDSCPage.Parse(childNode, namespaceManager));
			}
		}
		return cT_Pages;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (page != null)
		{
			foreach (CT_PCDSCPage item in page)
			{
				item.Write(sw, "page");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Pages()
	{
		pageField = new List<CT_PCDSCPage>();
	}
}
