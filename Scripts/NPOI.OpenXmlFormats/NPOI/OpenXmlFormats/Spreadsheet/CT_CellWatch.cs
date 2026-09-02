using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_CellWatch
{
	private string rField;

	public string r
	{
		get
		{
			return rField;
		}
		set
		{
			rField = value;
		}
	}

	public static CT_CellWatch Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_CellWatch
		{
			r = XmlHelper.ReadString(node.Attributes["r"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "r", r);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
