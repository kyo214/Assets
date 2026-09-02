using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_MemberProperty
{
	private string nameField;

	private bool showCellField;

	private bool showTipField;

	private bool showAsCaptionField;

	private uint nameLenField;

	private bool nameLenFieldSpecified;

	private uint pPosField;

	private bool pPosFieldSpecified;

	private uint pLenField;

	private bool pLenFieldSpecified;

	private uint levelField;

	private bool levelFieldSpecified;

	private uint fieldField;

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
	[DefaultValue(false)]
	public bool showCell
	{
		get
		{
			return showCellField;
		}
		set
		{
			showCellField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showTip
	{
		get
		{
			return showTipField;
		}
		set
		{
			showTipField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showAsCaption
	{
		get
		{
			return showAsCaptionField;
		}
		set
		{
			showAsCaptionField = value;
		}
	}

	[XmlAttribute]
	public uint nameLen
	{
		get
		{
			return nameLenField;
		}
		set
		{
			nameLenField = value;
		}
	}

	[XmlIgnore]
	public bool nameLenSpecified
	{
		get
		{
			return nameLenFieldSpecified;
		}
		set
		{
			nameLenFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint pPos
	{
		get
		{
			return pPosField;
		}
		set
		{
			pPosField = value;
		}
	}

	[XmlIgnore]
	public bool pPosSpecified
	{
		get
		{
			return pPosFieldSpecified;
		}
		set
		{
			pPosFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint pLen
	{
		get
		{
			return pLenField;
		}
		set
		{
			pLenField = value;
		}
	}

	[XmlIgnore]
	public bool pLenSpecified
	{
		get
		{
			return pLenFieldSpecified;
		}
		set
		{
			pLenFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint level
	{
		get
		{
			return levelField;
		}
		set
		{
			levelField = value;
		}
	}

	[XmlIgnore]
	public bool levelSpecified
	{
		get
		{
			return levelFieldSpecified;
		}
		set
		{
			levelFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint field
	{
		get
		{
			return fieldField;
		}
		set
		{
			fieldField = value;
		}
	}

	public static CT_MemberProperty Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_MemberProperty cT_MemberProperty = new CT_MemberProperty();
		cT_MemberProperty.name = XmlHelper.ReadString(node.Attributes["name"]);
		if (node.Attributes["showCell"] != null)
		{
			cT_MemberProperty.showCell = XmlHelper.ReadBool(node.Attributes["showCell"]);
		}
		if (node.Attributes["showTip"] != null)
		{
			cT_MemberProperty.showTip = XmlHelper.ReadBool(node.Attributes["showTip"]);
		}
		if (node.Attributes["showAsCaption"] != null)
		{
			cT_MemberProperty.showAsCaption = XmlHelper.ReadBool(node.Attributes["showAsCaption"]);
		}
		if (node.Attributes["nameLen"] != null)
		{
			cT_MemberProperty.nameLen = XmlHelper.ReadUInt(node.Attributes["nameLen"]);
		}
		if (node.Attributes["pPos"] != null)
		{
			cT_MemberProperty.pPos = XmlHelper.ReadUInt(node.Attributes["pPos"]);
		}
		if (node.Attributes["pLen"] != null)
		{
			cT_MemberProperty.pLen = XmlHelper.ReadUInt(node.Attributes["pLen"]);
		}
		if (node.Attributes["level"] != null)
		{
			cT_MemberProperty.level = XmlHelper.ReadUInt(node.Attributes["level"]);
		}
		if (node.Attributes["field"] != null)
		{
			cT_MemberProperty.field = XmlHelper.ReadUInt(node.Attributes["field"]);
		}
		return cT_MemberProperty;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "showCell", showCell);
		XmlHelper.WriteAttribute(sw, "showTip", showTip);
		XmlHelper.WriteAttribute(sw, "showAsCaption", showAsCaption);
		XmlHelper.WriteAttribute(sw, "nameLen", nameLen);
		XmlHelper.WriteAttribute(sw, "pPos", pPos);
		XmlHelper.WriteAttribute(sw, "pLen", pLen);
		XmlHelper.WriteAttribute(sw, "level", level);
		XmlHelper.WriteAttribute(sw, "field", field);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_MemberProperty()
	{
		showCellField = false;
		showTipField = false;
		showAsCaptionField = false;
	}
}
