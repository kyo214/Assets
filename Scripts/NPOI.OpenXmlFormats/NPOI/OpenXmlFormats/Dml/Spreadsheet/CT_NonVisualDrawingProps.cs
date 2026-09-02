using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing")]
public class CT_NonVisualDrawingProps
{
	private CT_Hyperlink hlinkClickField;

	private CT_Hyperlink hlinkHoverField;

	private CT_OfficeArtExtensionList extLstField;

	private uint idField;

	private string nameField;

	private string descrField;

	private bool? hiddenField;

	[XmlElement(Order = 0)]
	public CT_Hyperlink hlinkClick
	{
		get
		{
			return hlinkClickField;
		}
		set
		{
			hlinkClickField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_Hyperlink hlinkHover
	{
		get
		{
			return hlinkHoverField;
		}
		set
		{
			hlinkHoverField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_OfficeArtExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
		}
	}

	[XmlAttribute]
	public uint id
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
	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("")]
	public string descr
	{
		get
		{
			if (descrField != null)
			{
				return descrField;
			}
			return "";
		}
		set
		{
			descrField = value;
		}
	}

	[XmlIgnore]
	public bool descrSpecified => descrField != null;

	[XmlAttribute]
	[DefaultValue(false)]
	public bool hidden
	{
		get
		{
			if (hiddenField.HasValue)
			{
				return hiddenField.Value;
			}
			return false;
		}
		set
		{
			hiddenField = value;
		}
	}

	[XmlIgnore]
	public bool hiddenSpecified => hiddenField.HasValue;

	public static CT_NonVisualDrawingProps Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_NonVisualDrawingProps cT_NonVisualDrawingProps = new CT_NonVisualDrawingProps();
		cT_NonVisualDrawingProps.id = XmlHelper.ReadUInt(node.Attributes["id"]);
		cT_NonVisualDrawingProps.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_NonVisualDrawingProps.descr = XmlHelper.ReadString(node.Attributes["descr"]);
		cT_NonVisualDrawingProps.hidden = XmlHelper.ReadBool(node.Attributes["hidden"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "hlinkClick")
			{
				cT_NonVisualDrawingProps.hlinkClick = CT_Hyperlink.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "hlinkHover")
			{
				cT_NonVisualDrawingProps.hlinkHover = CT_Hyperlink.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_NonVisualDrawingProps.extLst = CT_OfficeArtExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_NonVisualDrawingProps;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<xdr:{nodeName}");
		XmlHelper.WriteAttribute(sw, "id", id, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "descr", descr);
		XmlHelper.WriteAttribute(sw, "hidden", hidden, writeIfBlank: false);
		sw.Write(">");
		if (hlinkClick != null)
		{
			hlinkClick.Write(sw, "hlinkClick");
		}
		if (hlinkHover != null)
		{
			hlinkHover.Write(sw, "hlinkHover");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</xdr:{nodeName}>");
	}
}
