using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Index
{
	private uint vField;

	public uint v
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

	public static CT_Index Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Index
		{
			v = XmlHelper.ReadUInt(node.Attributes["v"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "v", v);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
