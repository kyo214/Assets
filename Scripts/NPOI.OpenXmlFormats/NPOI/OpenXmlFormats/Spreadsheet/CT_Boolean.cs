using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Boolean
{
	private List<CT_X> xField;

	private bool vField;

	private bool uField;

	private bool uFieldSpecified;

	private bool fField;

	private bool fFieldSpecified;

	private string cField;

	private uint cpField;

	private bool cpFieldSpecified;

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
	public bool v
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

	public static CT_Boolean Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Boolean cT_Boolean = new CT_Boolean();
		if (node.Attributes["v"] != null)
		{
			cT_Boolean.v = XmlHelper.ReadBool(node.Attributes["v"]);
		}
		if (node.Attributes["u"] != null)
		{
			cT_Boolean.u = XmlHelper.ReadBool(node.Attributes["u"]);
		}
		if (node.Attributes["f"] != null)
		{
			cT_Boolean.f = XmlHelper.ReadBool(node.Attributes["f"]);
		}
		cT_Boolean.c = XmlHelper.ReadString(node.Attributes["c"]);
		if (node.Attributes["cp"] != null)
		{
			cT_Boolean.cp = XmlHelper.ReadUInt(node.Attributes["cp"]);
		}
		cT_Boolean.x = new List<CT_X>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "x")
			{
				cT_Boolean.x.Add(CT_X.Parse(childNode, namespaceManager));
			}
		}
		return cT_Boolean;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "v", v);
		XmlHelper.WriteAttribute(sw, "u", u);
		XmlHelper.WriteAttribute(sw, "f", f);
		XmlHelper.WriteAttribute(sw, "c", c);
		XmlHelper.WriteAttribute(sw, "cp", cp);
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

	public CT_Boolean()
	{
		xField = new List<CT_X>();
	}
}
