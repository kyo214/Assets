using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_OleSize
{
	private string refField;

	[XmlAttribute]
	public string @ref
	{
		get
		{
			return refField;
		}
		set
		{
			refField = value;
		}
	}

	public static CT_OleSize Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_OleSize
		{
			@ref = XmlHelper.ReadString(node.Attributes["ref"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "ref", @ref);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
