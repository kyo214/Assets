using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Borders
{
	private List<CT_Border> borderField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement]
	public List<CT_Border> border
	{
		get
		{
			return borderField;
		}
		set
		{
			borderField = value;
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

	public static CT_Borders Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Borders cT_Borders = new CT_Borders();
		cT_Borders.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		cT_Borders.border = new List<CT_Border>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "border")
			{
				cT_Borders.border.Add(CT_Border.Parse(childNode, namespaceManager));
			}
		}
		return cT_Borders;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count, writeIfBlank: true);
		sw.Write(">");
		if (border != null)
		{
			foreach (CT_Border item in border)
			{
				item.Write(sw, "border");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Border AddNewBorder()
	{
		if (borderField == null)
		{
			borderField = new List<CT_Border>();
		}
		CT_Border cT_Border = new CT_Border();
		borderField.Add(cT_Border);
		return cT_Border;
	}

	public void SetBorderArray(List<CT_Border> array)
	{
		borderField = array;
	}
}
