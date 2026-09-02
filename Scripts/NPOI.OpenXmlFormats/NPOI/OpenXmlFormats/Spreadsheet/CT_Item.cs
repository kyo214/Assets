using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Item
{
	private string nField;

	private ST_ItemType tField;

	private bool hField;

	private bool sField;

	private bool sdField;

	private bool fField;

	private bool mField;

	private bool cField;

	private uint xField;

	private bool xFieldSpecified;

	private bool dField;

	private bool eField;

	[XmlAttribute]
	public string n
	{
		get
		{
			return nField;
		}
		set
		{
			nField = value;
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
	[DefaultValue(false)]
	public bool h
	{
		get
		{
			return hField;
		}
		set
		{
			hField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool s
	{
		get
		{
			return sField;
		}
		set
		{
			sField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool sd
	{
		get
		{
			return sdField;
		}
		set
		{
			sdField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool f
	{
		get
		{
			return fField;
		}
		set
		{
			fField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool m
	{
		get
		{
			return mField;
		}
		set
		{
			mField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool c
	{
		get
		{
			return cField;
		}
		set
		{
			cField = value;
		}
	}

	[XmlAttribute]
	public uint x
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

	[XmlIgnore]
	public bool xSpecified
	{
		get
		{
			return xFieldSpecified;
		}
		set
		{
			xFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool d
	{
		get
		{
			return dField;
		}
		set
		{
			dField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool e
	{
		get
		{
			return eField;
		}
		set
		{
			eField = value;
		}
	}

	public static CT_Item Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Item cT_Item = new CT_Item();
		cT_Item.n = XmlHelper.ReadString(node.Attributes["n"]);
		if (node.Attributes["t"] != null)
		{
			cT_Item.t = (ST_ItemType)Enum.Parse(typeof(ST_ItemType), node.Attributes["t"].Value);
		}
		if (node.Attributes["h"] != null)
		{
			cT_Item.h = XmlHelper.ReadBool(node.Attributes["h"]);
		}
		if (node.Attributes["s"] != null)
		{
			cT_Item.s = XmlHelper.ReadBool(node.Attributes["s"]);
		}
		if (node.Attributes["sd"] != null)
		{
			cT_Item.sd = XmlHelper.ReadBool(node.Attributes["sd"]);
		}
		if (node.Attributes["f"] != null)
		{
			cT_Item.f = XmlHelper.ReadBool(node.Attributes["f"]);
		}
		if (node.Attributes["m"] != null)
		{
			cT_Item.m = XmlHelper.ReadBool(node.Attributes["m"]);
		}
		if (node.Attributes["c"] != null)
		{
			cT_Item.c = XmlHelper.ReadBool(node.Attributes["c"]);
		}
		if (node.Attributes["x"] != null)
		{
			cT_Item.x = XmlHelper.ReadUInt(node.Attributes["x"]);
		}
		if (node.Attributes["d"] != null)
		{
			cT_Item.d = XmlHelper.ReadBool(node.Attributes["d"]);
		}
		if (node.Attributes["e"] != null)
		{
			cT_Item.e = XmlHelper.ReadBool(node.Attributes["e"]);
		}
		return cT_Item;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "n", n);
		XmlHelper.WriteAttribute(sw, "t", t.ToString());
		XmlHelper.WriteAttribute(sw, "h", h);
		XmlHelper.WriteAttribute(sw, "s", s);
		XmlHelper.WriteAttribute(sw, "sd", sd);
		XmlHelper.WriteAttribute(sw, "f", f);
		XmlHelper.WriteAttribute(sw, "m", m);
		XmlHelper.WriteAttribute(sw, "c", c);
		XmlHelper.WriteAttribute(sw, "x", x);
		XmlHelper.WriteAttribute(sw, "d", d);
		XmlHelper.WriteAttribute(sw, "e", e);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_Item()
	{
		tField = ST_ItemType.data;
		hField = false;
		sField = false;
		sdField = true;
		fField = false;
		mField = false;
		cField = false;
		dField = false;
		eField = true;
	}
}
