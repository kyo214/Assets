using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_PictureFormat
{
	private ST_PictureFormat valField;

	[XmlAttribute]
	public ST_PictureFormat val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	public static CT_PictureFormat Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PictureFormat cT_PictureFormat = new CT_PictureFormat();
		if (node.Attributes["val"] != null)
		{
			cT_PictureFormat.val = (ST_PictureFormat)Enum.Parse(typeof(ST_PictureFormat), node.Attributes["val"].Value);
		}
		return cT_PictureFormat;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
