using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_RgbColor
{
	private byte[] rgbField;

	[XmlAttribute(DataType = "hexBinary")]
	public byte[] rgb
	{
		get
		{
			return rgbField;
		}
		set
		{
			rgbField = value;
		}
	}

	public static CT_RgbColor Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_RgbColor
		{
			rgb = XmlHelper.ReadBytes(node.Attributes["rgb"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "rgb", rgb);
		sw.Write("/>");
	}
}
