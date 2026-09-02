using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public class CT_FilterColumn
{
	private uint colIdField;

	private bool hiddenButtonField;

	private bool showButtonField;

	[XmlAttribute]
	public uint colId
	{
		get
		{
			return colIdField;
		}
		set
		{
			colIdField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool hiddenButton
	{
		get
		{
			return hiddenButtonField;
		}
		set
		{
			hiddenButtonField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showButton
	{
		get
		{
			return showButtonField;
		}
		set
		{
			showButtonField = value;
		}
	}

	public CT_FilterColumn()
	{
		hiddenButtonField = false;
		showButtonField = true;
	}

	public static CT_FilterColumn Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_FilterColumn
		{
			colId = XmlHelper.ReadUInt(node.Attributes["colId"]),
			hiddenButton = XmlHelper.ReadBool(node.Attributes["hiddenButton"]),
			showButton = XmlHelper.ReadBool(node.Attributes["showButton"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "colId", colId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "hiddenButton", hiddenButton);
		XmlHelper.WriteAttribute(sw, "showButton", showButton);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
