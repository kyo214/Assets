using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_Border
{
	private ST_Border valField;

	private string colorField;

	private ST_ThemeColor themeColorField;

	private bool themeColorFieldSpecified;

	private byte[] themeTintField;

	private byte[] themeShadeField;

	private ulong? szField;

	private ulong? spaceField;

	private ST_OnOff shadowField;

	private bool shadowFieldSpecified;

	private ST_OnOff frameField;

	private bool frameFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_Border val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string color
	{
		get
		{
			return colorField;
		}
		set
		{
			colorField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_ThemeColor themeColor
	{
		get
		{
			return themeColorField;
		}
		set
		{
			themeColorField = value;
		}
	}

	[XmlIgnore]
	public bool themeColorSpecified
	{
		get
		{
			return themeColorFieldSpecified;
		}
		set
		{
			themeColorFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "hexBinary")]
	public byte[] themeTint
	{
		get
		{
			return themeTintField;
		}
		set
		{
			themeTintField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "hexBinary")]
	public byte[] themeShade
	{
		get
		{
			return themeShadeField;
		}
		set
		{
			themeShadeField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong? sz
	{
		get
		{
			return szField;
		}
		set
		{
			szField = value;
		}
	}

	public ulong? space
	{
		get
		{
			return spaceField;
		}
		set
		{
			spaceField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff shadow
	{
		get
		{
			return shadowField;
		}
		set
		{
			shadowField = value;
		}
	}

	[XmlIgnore]
	public bool shadowSpecified
	{
		get
		{
			return shadowFieldSpecified;
		}
		set
		{
			shadowFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff frame
	{
		get
		{
			return frameField;
		}
		set
		{
			frameField = value;
		}
	}

	[XmlIgnore]
	public bool frameSpecified
	{
		get
		{
			return frameFieldSpecified;
		}
		set
		{
			frameFieldSpecified = value;
		}
	}

	public CT_Border()
	{
		themeColor = ST_ThemeColor.none;
	}

	public static CT_Border Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Border cT_Border = new CT_Border();
		if (node.Attributes["w:val"] != null)
		{
			cT_Border.val = (ST_Border)Enum.Parse(typeof(ST_Border), node.Attributes["w:val"].Value);
		}
		cT_Border.color = XmlHelper.ReadString(node.Attributes["w:color"]);
		if (node.Attributes["w:themeColor"] != null)
		{
			cT_Border.themeColor = (ST_ThemeColor)Enum.Parse(typeof(ST_ThemeColor), node.Attributes["w:themeColor"].Value);
		}
		cT_Border.themeTint = XmlHelper.ReadBytes(node.Attributes["w:themeTint"]);
		cT_Border.themeShade = XmlHelper.ReadBytes(node.Attributes["w:themeShade"]);
		if (node.Attributes["w:sz"] != null)
		{
			cT_Border.sz = XmlHelper.ReadULong(node.Attributes["w:sz"]);
		}
		if (node.Attributes["w:space"] != null)
		{
			cT_Border.space = XmlHelper.ReadULong(node.Attributes["w:space"]);
		}
		if (node.Attributes["w:shadow"] != null)
		{
			cT_Border.shadow = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:shadow"].Value, ignoreCase: true);
		}
		if (node.Attributes["w:frame"] != null)
		{
			cT_Border.frame = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:frame"].Value, ignoreCase: true);
		}
		return cT_Border;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val.ToString());
		XmlHelper.WriteAttribute(sw, "w:color", color);
		if (sz.HasValue)
		{
			XmlHelper.WriteAttribute(sw, "w:sz", sz.Value, writeIfBlank: true);
		}
		if (space.HasValue)
		{
			XmlHelper.WriteAttribute(sw, "w:space", space.Value, writeIfBlank: true);
		}
		if (themeColor != ST_ThemeColor.none)
		{
			XmlHelper.WriteAttribute(sw, "w:themeColor", themeColor.ToString());
		}
		XmlHelper.WriteAttribute(sw, "w:themeTint", themeTint);
		XmlHelper.WriteAttribute(sw, "w:themeShade", themeShade);
		if (shadow != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:shadow", shadow.ToString());
		}
		if (frame != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:frame", frame.ToString());
		}
		sw.Write("/>");
	}
}
