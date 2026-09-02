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
public class CT_RadarStyle
{
	private ST_RadarStyle valField;

	[XmlAttribute]
	[DefaultValue(ST_RadarStyle.standard)]
	public ST_RadarStyle val
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

	public CT_RadarStyle()
	{
		valField = ST_RadarStyle.standard;
	}

	public static CT_RadarStyle Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RadarStyle cT_RadarStyle = new CT_RadarStyle();
		if (node.Attributes["val"] != null)
		{
			cT_RadarStyle.val = (ST_RadarStyle)Enum.Parse(typeof(ST_RadarStyle), node.Attributes["val"].Value);
		}
		return cT_RadarStyle;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
