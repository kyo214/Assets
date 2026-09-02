using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_AutoSortScope
{
	private CT_PivotArea pivotAreaField;

	[XmlElement(Order = 0)]
	public CT_PivotArea pivotArea
	{
		get
		{
			return pivotAreaField;
		}
		set
		{
			pivotAreaField = value;
		}
	}

	public static CT_AutoSortScope Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_AutoSortScope cT_AutoSortScope = new CT_AutoSortScope();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotArea")
			{
				cT_AutoSortScope.pivotArea = CT_PivotArea.Parse(childNode, namespaceManager);
			}
		}
		return cT_AutoSortScope;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		sw.Write(">");
		if (pivotArea != null)
		{
			pivotArea.Write(sw, "pivotArea");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_AutoSortScope()
	{
		pivotAreaField = new CT_PivotArea();
	}
}
