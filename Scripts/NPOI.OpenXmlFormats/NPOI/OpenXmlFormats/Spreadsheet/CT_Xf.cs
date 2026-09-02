using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(ElementName = "xf", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public class CT_Xf
{
	private CT_CellAlignment alignmentField;

	private CT_CellProtection protectionField;

	private CT_ExtensionList extLstField;

	private uint numFmtIdField;

	private uint fontIdField;

	private uint fillIdField;

	private uint borderIdField;

	private uint xfIdField;

	private bool quotePrefixField;

	private bool pivotButtonField;

	private bool applyNumberFormatField;

	private bool applyFontField;

	private bool applyFillField;

	private bool applyBorderField;

	private bool applyAlignmentField;

	private bool applyProtectionField;

	private bool numFmtIdSpecifiedField;

	private bool fontIdSpecifiedField;

	private bool fillIdSpecifiedField;

	private bool borderIdSpecifiedField;

	private bool xfIdSpecifiedField;

	[XmlElement]
	public CT_CellAlignment alignment
	{
		get
		{
			return alignmentField;
		}
		set
		{
			alignmentField = value;
		}
	}

	[XmlElement]
	public CT_CellProtection protection
	{
		get
		{
			return protectionField;
		}
		set
		{
			protectionField = value;
		}
	}

	[XmlElement]
	public CT_ExtensionList extLst
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
	public uint numFmtId
	{
		get
		{
			return numFmtIdField;
		}
		set
		{
			numFmtIdField = value;
			numFmtIdSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool numFmtIdSpecified
	{
		get
		{
			return numFmtIdSpecifiedField;
		}
		set
		{
			numFmtIdSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint fontId
	{
		get
		{
			return fontIdField;
		}
		set
		{
			fontIdField = value;
			fontIdSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool fontIdSpecified
	{
		get
		{
			return fontIdSpecifiedField;
		}
		set
		{
			fontIdSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint fillId
	{
		get
		{
			return fillIdField;
		}
		set
		{
			fillIdField = value;
			fillIdSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool fillIdSpecified
	{
		get
		{
			return fillIdSpecifiedField;
		}
		set
		{
			fillIdSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint borderId
	{
		get
		{
			return borderIdField;
		}
		set
		{
			borderIdField = value;
			borderIdSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool borderIdSpecified
	{
		get
		{
			return borderIdSpecifiedField;
		}
		set
		{
			borderIdSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint xfId
	{
		get
		{
			return xfIdField;
		}
		set
		{
			xfIdField = value;
			xfIdSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool xfIdSpecified
	{
		get
		{
			return xfIdSpecifiedField;
		}
		set
		{
			xfIdSpecifiedField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool quotePrefix
	{
		get
		{
			return quotePrefixField;
		}
		set
		{
			quotePrefixField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool pivotButton
	{
		get
		{
			return pivotButtonField;
		}
		set
		{
			pivotButtonField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool applyNumberFormat
	{
		get
		{
			return applyNumberFormatField;
		}
		set
		{
			applyNumberFormatField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool applyFont
	{
		get
		{
			return applyFontField;
		}
		set
		{
			applyFontField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool applyFill
	{
		get
		{
			return applyFillField;
		}
		set
		{
			applyFillField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool applyBorder
	{
		get
		{
			return applyBorderField;
		}
		set
		{
			applyBorderField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool applyAlignment
	{
		get
		{
			return applyAlignmentField;
		}
		set
		{
			applyAlignmentField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool applyProtection
	{
		get
		{
			return applyProtectionField;
		}
		set
		{
			applyProtectionField = value;
		}
	}

	public CT_Xf Copy()
	{
		CT_Xf cT_Xf = new CT_Xf();
		if (alignment != null)
		{
			cT_Xf.alignment = alignment.Copy();
		}
		cT_Xf.protection = protection;
		cT_Xf.extLstField = ((extLstField == null) ? null : extLstField.Copy());
		cT_Xf.applyAlignment = applyAlignment;
		cT_Xf.applyBorder = applyBorder;
		cT_Xf.applyFill = applyFill;
		cT_Xf.applyFont = applyFont;
		cT_Xf.applyNumberFormat = applyNumberFormat;
		cT_Xf.applyProtection = applyProtection;
		cT_Xf.borderId = borderId;
		cT_Xf.borderIdSpecified = borderIdSpecified;
		cT_Xf.fillId = fillId;
		cT_Xf.fillIdSpecifiedField = fillIdSpecifiedField;
		cT_Xf.fontId = fontId;
		cT_Xf.fontIdSpecified = fontIdSpecified;
		cT_Xf.numFmtId = numFmtId;
		cT_Xf.numFmtIdSpecified = numFmtIdSpecified;
		cT_Xf.pivotButtonField = pivotButtonField;
		cT_Xf.quotePrefixField = quotePrefixField;
		cT_Xf.xfIdField = xfIdField;
		return cT_Xf;
	}

	public static CT_Xf Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Xf cT_Xf = new CT_Xf();
		cT_Xf.numFmtId = XmlHelper.ReadUInt(node.Attributes["numFmtId"]);
		cT_Xf.fontId = XmlHelper.ReadUInt(node.Attributes["fontId"]);
		cT_Xf.fillId = XmlHelper.ReadUInt(node.Attributes["fillId"]);
		cT_Xf.borderId = XmlHelper.ReadUInt(node.Attributes["borderId"]);
		cT_Xf.xfId = XmlHelper.ReadUInt(node.Attributes["xfId"]);
		cT_Xf.quotePrefix = XmlHelper.ReadBool(node.Attributes["quotePrefix"]);
		cT_Xf.pivotButton = XmlHelper.ReadBool(node.Attributes["pivotButton"]);
		cT_Xf.applyNumberFormat = XmlHelper.ReadBool(node.Attributes["applyNumberFormat"]);
		cT_Xf.applyFont = XmlHelper.ReadBool(node.Attributes["applyFont"]);
		cT_Xf.applyFill = XmlHelper.ReadBool(node.Attributes["applyFill"]);
		cT_Xf.applyBorder = XmlHelper.ReadBool(node.Attributes["applyBorder"]);
		cT_Xf.applyAlignment = XmlHelper.ReadBool(node.Attributes["applyAlignment"]);
		cT_Xf.applyProtection = XmlHelper.ReadBool(node.Attributes["applyProtection"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "alignment")
			{
				cT_Xf.alignment = CT_CellAlignment.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "protection")
			{
				cT_Xf.protection = CT_CellProtection.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_Xf.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_Xf;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "numFmtId", numFmtId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "fontId", fontId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "fillId", fillId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "borderId", borderId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "xfId", xfId, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "quotePrefix", quotePrefix, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "pivotButton", pivotButton, writeIfBlank: false);
		if (applyNumberFormat)
		{
			XmlHelper.WriteAttribute(sw, "applyNumberFormat", applyNumberFormat);
		}
		XmlHelper.WriteAttribute(sw, "applyFont", applyFont, writeIfBlank: false);
		if (applyFill)
		{
			XmlHelper.WriteAttribute(sw, "applyFill", applyFill);
		}
		if (applyBorder)
		{
			XmlHelper.WriteAttribute(sw, "applyBorder", applyBorder, writeIfBlank: true);
		}
		if (applyAlignment)
		{
			XmlHelper.WriteAttribute(sw, "applyAlignment", applyAlignment, writeIfBlank: true);
		}
		if (applyProtection)
		{
			XmlHelper.WriteAttribute(sw, "applyProtection", applyProtection, writeIfBlank: true);
		}
		if (alignment == null && protection == null && extLst == null)
		{
			sw.Write("/>");
			return;
		}
		sw.Write(">");
		if (alignment != null)
		{
			alignment.Write(sw, "alignment");
		}
		if (protection != null)
		{
			protection.Write(sw, "protection");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public override string ToString()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(CT_Xf));
		using StringWriter stringWriter = new StringWriter();
		xmlSerializer.Serialize(stringWriter, this);
		return stringWriter.ToString();
	}

	public bool IsSetFontId()
	{
		return fontIdSpecifiedField;
	}

	public bool IsSetAlignment()
	{
		return alignmentField != null;
	}

	public void UnsetAlignment()
	{
		alignmentField = null;
	}

	public bool IsSetExtLst()
	{
		return extLst == null;
	}

	public void UnsetExtLst()
	{
		extLst = null;
	}

	public bool IsSetProtection()
	{
		return protection != null;
	}

	public void UnsetProtection()
	{
		protection = null;
	}

	public bool IsSetLocked()
	{
		if (IsSetProtection())
		{
			return protectionField.locked;
		}
		return false;
	}

	public CT_CellProtection AddNewProtection()
	{
		protectionField = new CT_CellProtection();
		return protectionField;
	}

	public bool IsSetApplyFill()
	{
		return applyFillField;
	}
}
