using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_CellXfs
{
	private List<CT_Xf> xfField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement]
	public List<CT_Xf> xf
	{
		get
		{
			return xfField;
		}
		set
		{
			xfField = value;
		}
	}

	[XmlAttribute]
	public uint count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
		}
	}

	[XmlIgnore]
	public bool countSpecified
	{
		get
		{
			return countFieldSpecified;
		}
		set
		{
			countFieldSpecified = value;
		}
	}

	public static CT_CellXfs Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CellXfs cT_CellXfs = new CT_CellXfs();
		cT_CellXfs.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		cT_CellXfs.xf = new List<CT_Xf>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "xf")
			{
				cT_CellXfs.xf.Add(CT_Xf.Parse(childNode, namespaceManager));
			}
		}
		return cT_CellXfs;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (xf != null)
		{
			foreach (CT_Xf item in xf)
			{
				item.Write(sw, "xf");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Xf AddNewXf()
	{
		if (xfField == null)
		{
			xfField = new List<CT_Xf>();
		}
		CT_Xf cT_Xf = new CT_Xf();
		xfField.Add(cT_Xf);
		return cT_Xf;
	}
}
