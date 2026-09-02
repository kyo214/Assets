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
public class CT_TickMark
{
	private ST_TickMark valField;

	[XmlAttribute]
	[DefaultValue(ST_TickMark.cross)]
	public ST_TickMark val
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

	public CT_TickMark()
	{
		valField = ST_TickMark.cross;
	}

	public static CT_TickMark Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TickMark cT_TickMark = new CT_TickMark();
		if (node.Attributes["val"] != null)
		{
			cT_TickMark.val = (ST_TickMark)Enum.Parse(typeof(ST_TickMark), node.Attributes["val"].Value);
		}
		return cT_TickMark;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write("/>");
	}
}
