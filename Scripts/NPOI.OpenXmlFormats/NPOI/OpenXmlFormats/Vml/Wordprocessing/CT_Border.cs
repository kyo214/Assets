using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;
using NPOI.OpenXmlFormats.Util;

namespace NPOI.OpenXmlFormats.Vml.Wordprocessing;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:word")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:word", IsNullable = true)]
public class CT_Border
{
	private ST_BorderType typeField;

	private bool typeFieldSpecified;

	private string widthField;

	private ST_BorderShadow shadowField;

	private bool shadowFieldSpecified;

	[XmlAttribute]
	public ST_BorderType type
	{
		get
		{
			return typeField;
		}
		set
		{
			typeField = value;
		}
	}

	[XmlIgnore]
	public bool typeSpecified
	{
		get
		{
			return typeFieldSpecified;
		}
		set
		{
			typeFieldSpecified = value;
		}
	}

	[XmlAttribute(DataType = "positiveInteger")]
	public string width
	{
		get
		{
			return widthField;
		}
		set
		{
			widthField = value;
		}
	}

	[XmlAttribute]
	public ST_BorderShadow shadow
	{
		get
		{
			return shadowField;
		}
		set
		{
			shadowField = value;
		}
	}

	[XmlIgnore]
	public bool shadowSpecified
	{
		get
		{
			return shadowFieldSpecified;
		}
		set
		{
			shadowFieldSpecified = value;
		}
	}

	public static CT_Border Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Border cT_Border = new CT_Border();
		if (node.Attributes["type"] != null)
		{
			cT_Border.type = (ST_BorderType)Enum.Parse(typeof(ST_BorderType), node.Attributes["type"].Value);
		}
		cT_Border.width = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["width"]);
		cT_Border.shadow = NPOI.OpenXmlFormats.Util.XmlHelper.ReadBorderShadow(node.Attributes["shadow"]);
		return cT_Border;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		if (type != ST_BorderType.none)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "type", type.ToString());
		}
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "width", width);
		NPOI.OpenXmlFormats.Util.XmlHelper.WriteAttribute(sw, "shadow", shadow);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
