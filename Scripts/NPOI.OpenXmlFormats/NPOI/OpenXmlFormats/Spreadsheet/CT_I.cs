using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_I
{
	private List<CT_X> xField;

	private ST_ItemType tField;

	private uint rField;

	private uint iField;

	[XmlElement("x", Order = 0)]
	public List<CT_X> x
	{
		get
		{
			return xField;
		}
		set
		{
			xField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_ItemType.data)]
	public ST_ItemType t
	{
		get
		{
			return tField;
		}
		set
		{
			tField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint r
	{
		get
		{
			return rField;
		}
		set
		{
			rField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint i
	{
		get
		{
			return iField;
		}
		set
		{
			iField = value;
		}
	}

	public static CT_I Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_I cT_I = new CT_I();
		if (node.Attributes["t"] != null)
		{
			cT_I.t = (ST_ItemType)Enum.Parse(typeof(ST_ItemType), node.Attributes["t"].Value);
		}
		if (node.Attributes["r"] != null)
		{
			cT_I.r = XmlHelper.ReadUInt(node.Attributes["r"]);
		}
		if (node.Attributes["i"] != null)
		{
			cT_I.i = XmlHelper.ReadUInt(node.Attributes["i"]);
		}
		cT_I.x = new List<CT_X>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "x")
			{
				cT_I.x.Add(CT_X.Parse(childNode, namespaceManager));
			}
		}
		return cT_I;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "t", t.ToString());
		XmlHelper.WriteAttribute(sw, "r", r);
		XmlHelper.WriteAttribute(sw, "i", i);
		sw.Write(">");
		if (x != null)
		{
			foreach (CT_X item in x)
			{
				item.Write(sw, "x");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_I()
	{
		xField = new List<CT_X>();
		tField = ST_ItemType.data;
		rField = 0u;
		iField = 0u;
	}
}
