using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_OutlinePr
{
	private bool applyStylesField;

	private bool summaryBelowField;

	private bool summaryRightField;

	private bool showOutlineSymbolsField;

	[DefaultValue(false)]
	public bool applyStyles
	{
		get
		{
			return applyStylesField;
		}
		set
		{
			applyStylesField = value;
		}
	}

	[DefaultValue(true)]
	public bool summaryBelow
	{
		get
		{
			return summaryBelowField;
		}
		set
		{
			summaryBelowField = value;
		}
	}

	[DefaultValue(true)]
	public bool summaryRight
	{
		get
		{
			return summaryRightField;
		}
		set
		{
			summaryRightField = value;
		}
	}

	[DefaultValue(true)]
	public bool showOutlineSymbols
	{
		get
		{
			return showOutlineSymbolsField;
		}
		set
		{
			showOutlineSymbolsField = value;
		}
	}

	public static CT_OutlinePr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_OutlinePr
		{
			applyStyles = XmlHelper.ReadBool(node.Attributes["applyStyles"]),
			summaryBelow = XmlHelper.ReadBool(node.Attributes["summaryBelow"]),
			summaryRight = XmlHelper.ReadBool(node.Attributes["summaryRight"]),
			showOutlineSymbols = XmlHelper.ReadBool(node.Attributes["showOutlineSymbols"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "applyStyles", applyStyles);
		XmlHelper.WriteAttribute(sw, "summaryBelow", summaryBelow);
		XmlHelper.WriteAttribute(sw, "summaryRight", summaryRight);
		XmlHelper.WriteAttribute(sw, "showOutlineSymbols", showOutlineSymbols);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_OutlinePr()
	{
		applyStylesField = false;
		summaryBelowField = true;
		summaryRightField = true;
		showOutlineSymbolsField = true;
	}

	public CT_OutlinePr Clone()
	{
		return new CT_OutlinePr
		{
			applyStylesField = applyStylesField,
			showOutlineSymbolsField = showOutlineSymbolsField,
			summaryBelowField = summaryBelowField,
			summaryRightField = summaryRightField
		};
	}
}
