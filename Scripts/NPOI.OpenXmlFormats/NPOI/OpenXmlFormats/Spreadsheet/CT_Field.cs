using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Field
{
	private int xField;

	[XmlAttribute]
	public int x
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

	public static CT_Field Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Field cT_Field = new CT_Field();
		if (node.Attributes["x"] != null)
		{
			cT_Field.x = XmlHelper.ReadInt(node.Attributes["x"]);
		}
		return cT_Field;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "x", x);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
