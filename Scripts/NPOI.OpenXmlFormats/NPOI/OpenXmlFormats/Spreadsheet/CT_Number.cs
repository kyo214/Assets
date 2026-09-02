using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Number
{
	private List<CT_Tuples> tplsField;

	private List<CT_X> xField;

	private double vField;

	private bool uField;

	private bool uFieldSpecified;

	private bool fField;

	private bool fFieldSpecified;

	private string cField;

	private uint cpField;

	private bool cpFieldSpecified;

	private uint inField;

	private bool inFieldSpecified;

	private byte[] bcField;

	private byte[] fcField;

	private bool iField;

	private bool unField;

	private bool stField;

	private bool bField;

	[XmlElement("tpls", Order = 0)]
	public List<CT_Tuples> tpls
	{
		get
		{
			return tplsField;
		}
		set
		{
			tplsField = value;
		}
	}

	[XmlElement("x", Order = 1)]
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
	public double v
	{
		get
		{
			return vField;
		}
		set
		{
			vField = value;
		}
	}

	[XmlAttribute]
	public bool u
	{
		get
		{
			return uField;
		}
		set
		{
			uField = value;
		}
	}

	[XmlIgnore]
	public bool uSpecified
	{
		get
		{
			return uFieldSpecified;
		}
		set
		{
			uFieldSpecified = value;
		}
	}

	[XmlAttribute]
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

	[XmlIgnore]
	public bool fSpecified
	{
		get
		{
			return fFieldSpecified;
		}
		set
		{
			fFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string c
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
	public uint cp
	{
		get
		{
			return cpField;
		}
		set
		{
			cpField = value;
		}
	}

	[XmlIgnore]
	public bool cpSpecified
	{
		get
		{
			return cpFieldSpecified;
		}
		set
		{
			cpFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint @in
	{
		get
		{
			return inField;
		}
		set
		{
			inField = value;
		}
	}

	[XmlIgnore]
	public bool inSpecified
	{
		get
		{
			return inFieldSpecified;
		}
		set
		{
			inFieldSpecified = value;
		}
	}

	[XmlAttribute(DataType = "hexBinary")]
	public byte[] bc
	{
		get
		{
			return bcField;
		}
		set
		{
			bcField = value;
		}
	}

	[XmlAttribute(DataType = "hexBinary")]
	public byte[] fc
	{
		get
		{
			return fcField;
		}
		set
		{
			fcField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool i
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

	[XmlAttribute]
	[DefaultValue(false)]
	public bool un
	{
		get
		{
			return unField;
		}
		set
		{
			unField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool st
	{
		get
		{
			return stField;
		}
		set
		{
			stField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool b
	{
		get
		{
			return bField;
		}
		set
		{
			bField = value;
		}
	}

	public CT_Number()
	{
		xField = new List<CT_X>();
		tplsField = new List<CT_Tuples>();
		iField = false;
		unField = false;
		stField = false;
		bField = false;
	}

	public static CT_Number Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Number cT_Number = new CT_Number();
		if (node.Attributes["v"] != null)
		{
			cT_Number.v = XmlHelper.ReadDouble(node.Attributes["v"]);
		}
		if (node.Attributes["u"] != null)
		{
			cT_Number.u = XmlHelper.ReadBool(node.Attributes["u"]);
		}
		if (node.Attributes["f"] != null)
		{
			cT_Number.f = XmlHelper.ReadBool(node.Attributes["f"]);
		}
		cT_Number.c = XmlHelper.ReadString(node.Attributes["c"]);
		if (node.Attributes["cp"] != null)
		{
			cT_Number.cp = XmlHelper.ReadUInt(node.Attributes["cp"]);
		}
		if (node.Attributes["in"] != null)
		{
			cT_Number.@in = XmlHelper.ReadUInt(node.Attributes["in"]);
		}
		cT_Number.bc = XmlHelper.ReadBytes(node.Attributes["bc"]);
		cT_Number.fc = XmlHelper.ReadBytes(node.Attributes["fc"]);
		if (node.Attributes["i"] != null)
		{
			cT_Number.i = XmlHelper.ReadBool(node.Attributes["i"]);
		}
		if (node.Attributes["un"] != null)
		{
			cT_Number.un = XmlHelper.ReadBool(node.Attributes["un"]);
		}
		if (node.Attributes["st"] != null)
		{
			cT_Number.st = XmlHelper.ReadBool(node.Attributes["st"]);
		}
		if (node.Attributes["b"] != null)
		{
			cT_Number.b = XmlHelper.ReadBool(node.Attributes["b"]);
		}
		cT_Number.tpls = new List<CT_Tuples>();
		cT_Number.x = new List<CT_X>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "tpls")
			{
				cT_Number.tpls.Add(CT_Tuples.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "x")
			{
				cT_Number.x.Add(CT_X.Parse(childNode, namespaceManager));
			}
		}
		return cT_Number;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "v", v);
		XmlHelper.WriteAttribute(sw, "u", u);
		XmlHelper.WriteAttribute(sw, "f", f);
		XmlHelper.WriteAttribute(sw, "c", c);
		XmlHelper.WriteAttribute(sw, "cp", cp);
		XmlHelper.WriteAttribute(sw, "in", @in);
		XmlHelper.WriteAttribute(sw, "bc", bc);
		XmlHelper.WriteAttribute(sw, "fc", fc);
		XmlHelper.WriteAttribute(sw, "i", i);
		XmlHelper.WriteAttribute(sw, "un", un);
		XmlHelper.WriteAttribute(sw, "st", st);
		XmlHelper.WriteAttribute(sw, "b", b);
		sw.Write(">");
		if (tpls != null)
		{
			foreach (CT_Tuples tpl in tpls)
			{
				tpl.Write(sw, "tpls");
			}
		}
		if (x != null)
		{
			foreach (CT_X item in x)
			{
				item.Write(sw, "x");
			}
		}
		sw.Write($"</{nodeName}>");
	}
}
