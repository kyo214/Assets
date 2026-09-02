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
public class CT_TrendlineType
{
	private ST_TrendlineType valField;

	[XmlAttribute]
	[DefaultValue(ST_TrendlineType.linear)]
	public ST_TrendlineType val
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

	public CT_TrendlineType()
	{
		valField = ST_TrendlineType.linear;
	}

	public static CT_TrendlineType Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TrendlineType cT_TrendlineType = new CT_TrendlineType();
		if (node.Attributes["val"] != null)
		{
			cT_TrendlineType.val = (ST_TrendlineType)Enum.Parse(typeof(ST_TrendlineType), node.Attributes["val"].Value);
		}
		return cT_TrendlineType;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
