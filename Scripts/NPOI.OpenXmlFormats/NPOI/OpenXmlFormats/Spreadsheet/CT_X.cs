using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_X
{
	private int vField;

	[XmlAttribute]
	[DefaultValue(0)]
	public int v
	{
		get
		{
			return vField;
		}
		set
		{
			vField = value;
		}
	}

	public CT_X()
	{
		vField = 0;
	}

	public static CT_X Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_X cT_X = new CT_X();
		if (node.Attributes["v"] != null)
		{
			cT_X.v = XmlHelper.ReadInt(node.Attributes["v"]);
		}
		return cT_X;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "v", v);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
