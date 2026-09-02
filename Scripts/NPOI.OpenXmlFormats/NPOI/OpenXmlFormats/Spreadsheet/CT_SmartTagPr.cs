using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_SmartTagPr
{
	private bool embedField;

	private ST_SmartTagShow showField;

	[XmlAttribute]
	[DefaultValue(false)]
	public bool embed
	{
		get
		{
			return embedField;
		}
		set
		{
			embedField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_SmartTagShow.all)]
	public ST_SmartTagShow show
	{
		get
		{
			return showField;
		}
		set
		{
			showField = value;
		}
	}

	public static CT_SmartTagPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_SmartTagPr cT_SmartTagPr = new CT_SmartTagPr();
		cT_SmartTagPr.embed = XmlHelper.ReadBool(node.Attributes["embed"]);
		if (node.Attributes["show"] != null)
		{
			cT_SmartTagPr.show = (ST_SmartTagShow)Enum.Parse(typeof(ST_SmartTagShow), node.Attributes["show"].Value);
		}
		return cT_SmartTagPr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "embed", embed);
		XmlHelper.WriteAttribute(sw, "show", show.ToString());
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_SmartTagPr()
	{
		embedField = false;
		showField = ST_SmartTagShow.all;
	}
}
