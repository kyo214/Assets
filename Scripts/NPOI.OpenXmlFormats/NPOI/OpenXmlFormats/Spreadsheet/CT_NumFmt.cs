using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_NumFmt
{
	private uint numFmtIdField;

	private string formatCodeField;

	[XmlAttribute]
	public uint numFmtId
	{
		get
		{
			return numFmtIdField;
		}
		set
		{
			numFmtIdField = value;
		}
	}

	[XmlAttribute]
	public string formatCode
	{
		get
		{
			return formatCodeField;
		}
		set
		{
			formatCodeField = value;
		}
	}

	public static CT_NumFmt Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_NumFmt
		{
			numFmtId = XmlHelper.ReadUInt(node.Attributes["numFmtId"]),
			formatCode = XmlHelper.ReadString(node.Attributes["formatCode"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "numFmtId", numFmtId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "formatCode", formatCode);
		sw.Write("/>");
	}
}
