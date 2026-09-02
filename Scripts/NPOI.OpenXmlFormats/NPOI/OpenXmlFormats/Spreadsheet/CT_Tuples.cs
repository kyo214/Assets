using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Tuples
{
	private List<CT_Tuple> tplField;

	private uint cField;

	private bool cFieldSpecified;

	[XmlElement("tpl", Order = 0)]
	public List<CT_Tuple> tpl
	{
		get
		{
			return tplField;
		}
		set
		{
			tplField = value;
		}
	}

	[XmlAttribute]
	public uint c
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

	[XmlIgnore]
	public bool cSpecified
	{
		get
		{
			return cFieldSpecified;
		}
		set
		{
			cFieldSpecified = value;
		}
	}

	public CT_Tuples()
	{
		tplField = new List<CT_Tuple>();
	}

	public static CT_Tuples Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Tuples cT_Tuples = new CT_Tuples();
		if (node.Attributes["c"] != null)
		{
			cT_Tuples.c = XmlHelper.ReadUInt(node.Attributes["c"]);
		}
		cT_Tuples.tpl = new List<CT_Tuple>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "tpl")
			{
				cT_Tuples.tpl.Add(CT_Tuple.Parse(childNode, namespaceManager));
			}
		}
		return cT_Tuples;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "c", c);
		sw.Write(">");
		if (tpl != null)
		{
			foreach (CT_Tuple item in tpl)
			{
				item.Write(sw, "tpl");
			}
		}
		sw.Write($"</{nodeName}>");
	}
}
