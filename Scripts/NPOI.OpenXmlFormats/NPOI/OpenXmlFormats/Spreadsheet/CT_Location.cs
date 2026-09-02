using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Location
{
	private string refField;

	private uint firstHeaderRowField;

	private uint firstDataRowField;

	private uint firstDataColField;

	private uint rowPageCountField;

	private uint colPageCountField;

	[XmlAttribute]
	public string @ref
	{
		get
		{
			return refField;
		}
		set
		{
			refField = value;
		}
	}

	[XmlAttribute]
	public uint firstHeaderRow
	{
		get
		{
			return firstHeaderRowField;
		}
		set
		{
			firstHeaderRowField = value;
		}
	}

	[XmlAttribute]
	public uint firstDataRow
	{
		get
		{
			return firstDataRowField;
		}
		set
		{
			firstDataRowField = value;
		}
	}

	[XmlAttribute]
	public uint firstDataCol
	{
		get
		{
			return firstDataColField;
		}
		set
		{
			firstDataColField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint rowPageCount
	{
		get
		{
			return rowPageCountField;
		}
		set
		{
			rowPageCountField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint colPageCount
	{
		get
		{
			return colPageCountField;
		}
		set
		{
			colPageCountField = value;
		}
	}

	public static CT_Location Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Location cT_Location = new CT_Location();
		cT_Location.@ref = XmlHelper.ReadString(node.Attributes["ref"]);
		if (node.Attributes["firstHeaderRow"] != null)
		{
			cT_Location.firstHeaderRow = XmlHelper.ReadUInt(node.Attributes["firstHeaderRow"]);
		}
		if (node.Attributes["firstDataRow"] != null)
		{
			cT_Location.firstDataRow = XmlHelper.ReadUInt(node.Attributes["firstDataRow"]);
		}
		if (node.Attributes["firstDataCol"] != null)
		{
			cT_Location.firstDataCol = XmlHelper.ReadUInt(node.Attributes["firstDataCol"]);
		}
		if (node.Attributes["rowPageCount"] != null)
		{
			cT_Location.rowPageCount = XmlHelper.ReadUInt(node.Attributes["rowPageCount"]);
		}
		if (node.Attributes["colPageCount"] != null)
		{
			cT_Location.colPageCount = XmlHelper.ReadUInt(node.Attributes["colPageCount"]);
		}
		return cT_Location;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "ref", @ref);
		XmlHelper.WriteAttribute(sw, "firstHeaderRow", firstHeaderRow);
		XmlHelper.WriteAttribute(sw, "firstDataRow", firstDataRow);
		XmlHelper.WriteAttribute(sw, "firstDataCol", firstDataCol);
		XmlHelper.WriteAttribute(sw, "rowPageCount", rowPageCount);
		XmlHelper.WriteAttribute(sw, "colPageCount", colPageCount);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_Location()
	{
		rowPageCountField = 0u;
		colPageCountField = 0u;
	}
}
