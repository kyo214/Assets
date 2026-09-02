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
public class CT_ScatterStyle
{
	private ST_ScatterStyle valField;

	[XmlAttribute]
	[DefaultValue(ST_ScatterStyle.marker)]
	public ST_ScatterStyle val
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

	public static CT_ScatterStyle Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ScatterStyle cT_ScatterStyle = new CT_ScatterStyle();
		if (node.Attributes["val"] != null)
		{
			cT_ScatterStyle.val = (ST_ScatterStyle)Enum.Parse(typeof(ST_ScatterStyle), node.Attributes["val"].Value);
		}
		return cT_ScatterStyle;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}

	public CT_ScatterStyle()
	{
		valField = ST_ScatterStyle.marker;
	}
}
