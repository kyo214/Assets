using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_RelativeRect
{
	private int? lField;

	private int? tField;

	private int? rField;

	private int? bField;

	[XmlAttribute]
	[DefaultValue(0)]
	public int l
	{
		get
		{
			if (!lField.HasValue)
			{
				return 0;
			}
			return lField.Value;
		}
		set
		{
			lField = value;
		}
	}

	[XmlIgnore]
	public bool lSpecified => lField.HasValue;

	[XmlAttribute]
	[DefaultValue(0)]
	public int t
	{
		get
		{
			if (!tField.HasValue)
			{
				return 0;
			}
			return tField.Value;
		}
		set
		{
			tField = value;
		}
	}

	[XmlIgnore]
	public bool tSpecified => tField.HasValue;

	[XmlAttribute]
	[DefaultValue(0)]
	public int r
	{
		get
		{
			if (!rField.HasValue)
			{
				return 0;
			}
			return rField.Value;
		}
		set
		{
			rField = value;
		}
	}

	[XmlIgnore]
	public bool rSpecified => rField.HasValue;

	[XmlAttribute]
	[DefaultValue(0)]
	public int b
	{
		get
		{
			if (!bField.HasValue)
			{
				return 0;
			}
			return bField.Value;
		}
		set
		{
			bField = value;
		}
	}

	[XmlIgnore]
	public bool bSpecified => bField.HasValue;

	public static CT_RelativeRect Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_RelativeRect
		{
			l = XmlHelper.ReadInt(node.Attributes["l"]),
			t = XmlHelper.ReadInt(node.Attributes["t"]),
			r = XmlHelper.ReadInt(node.Attributes["r"]),
			b = XmlHelper.ReadInt(node.Attributes["b"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "l", l);
		XmlHelper.WriteAttribute(sw, "t", t);
		XmlHelper.WriteAttribute(sw, "r", r);
		XmlHelper.WriteAttribute(sw, "b", b);
		sw.Write("/>");
	}
}
