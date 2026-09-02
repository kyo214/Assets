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
public class CT_TimeUnit
{
	private ST_TimeUnit valField;

	[XmlAttribute]
	[DefaultValue(ST_TimeUnit.days)]
	public ST_TimeUnit val
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

	public static CT_TimeUnit Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TimeUnit cT_TimeUnit = new CT_TimeUnit();
		if (node.Attributes["val"] != null)
		{
			cT_TimeUnit.val = (ST_TimeUnit)Enum.Parse(typeof(ST_TimeUnit), node.Attributes["val"].Value);
		}
		return cT_TimeUnit;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}

	public CT_TimeUnit()
	{
		valField = ST_TimeUnit.days;
	}
}
