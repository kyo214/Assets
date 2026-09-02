using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Hyperlink
{
	private string refField;

	private string idField;

	private string locationField;

	private string tooltipField;

	private string displayField;

	[XmlAttribute("ref")]
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

	[XmlAttribute(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships")]
	public string id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	[XmlAttribute]
	public string location
	{
		get
		{
			return locationField;
		}
		set
		{
			locationField = value;
		}
	}

	[XmlAttribute]
	public string tooltip
	{
		get
		{
			return tooltipField;
		}
		set
		{
			tooltipField = value;
		}
	}

	[XmlAttribute]
	public string display
	{
		get
		{
			return displayField;
		}
		set
		{
			displayField = value;
		}
	}

	public static CT_Hyperlink Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Hyperlink
		{
			@ref = XmlHelper.ReadString(node.Attributes["ref"]),
			id = XmlHelper.ReadString(node.Attributes["id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships"]),
			location = XmlHelper.ReadString(node.Attributes["location"]),
			tooltip = XmlHelper.ReadString(node.Attributes["tooltip"]),
			display = XmlHelper.ReadString(node.Attributes["display"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "ref", @ref);
		XmlHelper.WriteAttribute(sw, "r:id", id);
		XmlHelper.WriteAttribute(sw, "location", location);
		XmlHelper.WriteAttribute(sw, "tooltip", tooltip);
		XmlHelper.WriteAttribute(sw, "display", display);
		sw.Write("/>");
	}

	public CT_Hyperlink Copy()
	{
		return new CT_Hyperlink
		{
			@ref = @ref,
			id = id,
			location = location,
			tooltip = tooltip,
			display = display
		};
	}
}
