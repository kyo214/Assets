using System;
using System.Collections.Generic;
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
public class CT_ExternalRow
{
	private List<CT_ExternalCell> cellField;

	private uint rField;

	[XmlElement("cell")]
	public CT_ExternalCell[] cell
	{
		get
		{
			return cellField.ToArray();
		}
		set
		{
			cellField.Clear();
			cellField.AddRange(value);
		}
	}

	[XmlAttribute]
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

	public CT_ExternalRow()
	{
		cellField = new List<CT_ExternalCell>();
	}

	internal static CT_ExternalRow Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalRow cT_ExternalRow = new CT_ExternalRow();
		cT_ExternalRow.r = XmlHelper.ReadUInt(node.Attributes["r"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "cell")
			{
				cT_ExternalRow.cellField.Add(CT_ExternalCell.Parse(childNode, namespaceManager));
			}
		}
		return cT_ExternalRow;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "r", rField);
		sw.Write(">");
		foreach (CT_ExternalCell item in cellField)
		{
			item.Write(sw, "cell");
		}
		sw.Write($"</{nodeName}>");
	}
}
