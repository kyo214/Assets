using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_HierarchyUsage
{
	private int hierarchyUsageField;

	[XmlAttribute]
	public int hierarchyUsage
	{
		get
		{
			return hierarchyUsageField;
		}
		set
		{
			hierarchyUsageField = value;
		}
	}

	public static CT_HierarchyUsage Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_HierarchyUsage cT_HierarchyUsage = new CT_HierarchyUsage();
		if (node.Attributes["hierarchyUsage"] != null)
		{
			cT_HierarchyUsage.hierarchyUsage = XmlHelper.ReadInt(node.Attributes["hierarchyUsage"]);
		}
		return cT_HierarchyUsage;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "hierarchyUsage", hierarchyUsage);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
