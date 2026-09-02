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
public class CT_TickLblPos
{
	private ST_TickLblPos valField;

	[XmlAttribute]
	[DefaultValue(ST_TickLblPos.nextTo)]
	public ST_TickLblPos val
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

	public CT_TickLblPos()
	{
		valField = ST_TickLblPos.nextTo;
	}

	public static CT_TickLblPos Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TickLblPos cT_TickLblPos = new CT_TickLblPos();
		if (node.Attributes["val"] != null)
		{
			cT_TickLblPos.val = (ST_TickLblPos)Enum.Parse(typeof(ST_TickLblPos), node.Attributes["val"].Value);
		}
		return cT_TickLblPos;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write("/>");
	}
}
