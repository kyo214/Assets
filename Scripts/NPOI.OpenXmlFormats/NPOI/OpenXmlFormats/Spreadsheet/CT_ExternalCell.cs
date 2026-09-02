using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ExternalCell
{
	private string vField;

	private string rField;

	private ST_CellType tField;

	private uint vmField;

	public string v
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
	public string r
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
	[DefaultValue(ST_CellType.n)]
	public ST_CellType t
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
	public uint vm
	{
		get
		{
			return vmField;
		}
		set
		{
			vmField = value;
		}
	}

	public CT_ExternalCell()
	{
		tField = ST_CellType.n;
		vmField = 0u;
	}

	internal static CT_ExternalCell Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalCell cT_ExternalCell = new CT_ExternalCell();
		cT_ExternalCell.rField = XmlHelper.ReadString(node.Attributes["r"]);
		if (node.Attributes["t"] != null)
		{
			cT_ExternalCell.tField = (ST_CellType)Enum.Parse(typeof(ST_CellType), node.Attributes["t"].Value);
		}
		cT_ExternalCell.vm = XmlHelper.ReadUInt(node.Attributes["vm"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "v")
			{
				cT_ExternalCell.v = childNode.InnerText;
			}
		}
		return cT_ExternalCell;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "r", rField);
		if (t != ST_CellType.n)
		{
			XmlHelper.WriteAttribute(sw, "t", tField.ToString());
		}
		XmlHelper.WriteAttribute(sw, "vm", vmField);
		if (v == null)
		{
			sw.Write("/>");
			return;
		}
		sw.Write(">");
		sw.Write($"<v>{XmlHelper.EncodeXml(v)}</v>");
		sw.Write($"</{nodeName}>");
	}
}
