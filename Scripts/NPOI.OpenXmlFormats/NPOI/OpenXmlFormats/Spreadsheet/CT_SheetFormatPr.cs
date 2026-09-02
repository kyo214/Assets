using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_SheetFormatPr
{
	private uint baseColWidthField;

	private double defaultColWidthField;

	private double defaultRowHeightField;

	private bool customHeightField;

	private bool zeroHeightField;

	private bool thickTopField;

	private bool thickBottomField;

	private byte outlineLevelRowField;

	private byte outlineLevelColField;

	private double dyDescentField;

	[XmlAttribute]
	[DefaultValue(typeof(uint), "8")]
	public uint baseColWidth
	{
		get
		{
			return baseColWidthField;
		}
		set
		{
			baseColWidthField = value;
		}
	}

	[XmlAttribute]
	public double defaultColWidth
	{
		get
		{
			return defaultColWidthField;
		}
		set
		{
			defaultColWidthField = value;
		}
	}

	[XmlAttribute]
	public double defaultRowHeight
	{
		get
		{
			return defaultRowHeightField;
		}
		set
		{
			defaultRowHeightField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool customHeight
	{
		get
		{
			return customHeightField;
		}
		set
		{
			customHeightField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool zeroHeight
	{
		get
		{
			return zeroHeightField;
		}
		set
		{
			zeroHeightField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool thickTop
	{
		get
		{
			return thickTopField;
		}
		set
		{
			thickTopField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool thickBottom
	{
		get
		{
			return thickBottomField;
		}
		set
		{
			thickBottomField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(byte), "0")]
	public byte outlineLevelRow
	{
		get
		{
			return outlineLevelRowField;
		}
		set
		{
			outlineLevelRowField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(byte), "0")]
	public byte outlineLevelCol
	{
		get
		{
			return outlineLevelColField;
		}
		set
		{
			outlineLevelColField = value;
		}
	}

	public CT_SheetFormatPr()
	{
		baseColWidth = 8u;
	}

	public static CT_SheetFormatPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_SheetFormatPr
		{
			baseColWidth = XmlHelper.ReadUInt(node.Attributes["baseColWidth"]),
			defaultColWidth = XmlHelper.ReadDouble(node.Attributes["defaultColWidth"]),
			defaultRowHeight = XmlHelper.ReadDouble(node.Attributes["defaultRowHeight"]),
			customHeight = XmlHelper.ReadBool(node.Attributes["customHeight"]),
			zeroHeight = XmlHelper.ReadBool(node.Attributes["zeroHeight"]),
			thickTop = XmlHelper.ReadBool(node.Attributes["thickTop"]),
			outlineLevelRow = XmlHelper.ReadByte(node.Attributes["outlineLevelRow"]),
			outlineLevelCol = XmlHelper.ReadByte(node.Attributes["outlineLevelCol"]),
			thickBottom = XmlHelper.ReadBool(node.Attributes["thickBottom"]),
			dyDescentField = XmlHelper.ReadDouble(node.Attributes["x14ac:dyDescent"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "baseColWidth", baseColWidth);
		XmlHelper.WriteAttribute(sw, "defaultColWidth", defaultColWidth);
		XmlHelper.WriteAttribute(sw, "defaultRowHeight", defaultRowHeight);
		XmlHelper.WriteAttribute(sw, "customHeight", customHeight, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "zeroHeight", zeroHeight, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "thickTop", thickTop, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "thickBottom", thickBottom, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "outlineLevelRow", outlineLevelRow);
		XmlHelper.WriteAttribute(sw, "outlineLevelCol", outlineLevelCol);
		XmlHelper.WriteAttribute(sw, "x14ac:dyDescent", dyDescentField, writeIfBlank: false);
		sw.Write("/>");
	}
}
