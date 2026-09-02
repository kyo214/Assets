using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Consolidation
{
	private CT_Pages pagesField;

	private CT_RangeSets rangeSetsField;

	private bool autoPageField;

	[XmlElement(Order = 0)]
	public CT_Pages pages
	{
		get
		{
			return pagesField;
		}
		set
		{
			pagesField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_RangeSets rangeSets
	{
		get
		{
			return rangeSetsField;
		}
		set
		{
			rangeSetsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool autoPage
	{
		get
		{
			return autoPageField;
		}
		set
		{
			autoPageField = value;
		}
	}

	public static CT_Consolidation Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Consolidation cT_Consolidation = new CT_Consolidation();
		if (node.Attributes["autoPage"] != null)
		{
			cT_Consolidation.autoPage = XmlHelper.ReadBool(node.Attributes["autoPage"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pages")
			{
				cT_Consolidation.pages = CT_Pages.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "rangeSets")
			{
				cT_Consolidation.rangeSets = CT_RangeSets.Parse(childNode, namespaceManager);
			}
		}
		return cT_Consolidation;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "autoPage", autoPage);
		sw.Write(">");
		if (pages != null)
		{
			pages.Write(sw, "pages");
		}
		if (rangeSets != null)
		{
			rangeSets.Write(sw, "rangeSets");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Consolidation()
	{
		rangeSetsField = new CT_RangeSets();
		pagesField = new CT_Pages();
		autoPageField = true;
	}
}
