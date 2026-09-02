using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Col
{
	private uint minField;

	private uint maxField;

	private double widthField;

	private bool widthSpecifiedField;

	private uint? styleField;

	private bool hiddenField;

	private bool bestFitField;

	private bool customWidthField;

	private bool phoneticField;

	private byte outlineLevelField;

	private bool collapsedField = true;

	private bool collapsedSpecifiedField = true;

	[XmlAttribute]
	public uint min
	{
		get
		{
			return minField;
		}
		set
		{
			minField = value;
		}
	}

	[XmlAttribute]
	public uint max
	{
		get
		{
			return maxField;
		}
		set
		{
			maxField = value;
		}
	}

	[XmlAttribute]
	public double width
	{
		get
		{
			return widthField;
		}
		set
		{
			widthField = value;
			widthSpecified = true;
		}
	}

	[XmlIgnore]
	public bool widthSpecified
	{
		get
		{
			return widthSpecifiedField;
		}
		set
		{
			widthSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint? style
	{
		get
		{
			return styleField;
		}
		set
		{
			styleField = value;
		}
	}

	[DefaultValue(false)]
	[XmlAttribute]
	public bool hidden
	{
		get
		{
			return hiddenField;
		}
		set
		{
			hiddenField = value;
		}
	}

	[DefaultValue(false)]
	[XmlAttribute]
	public bool bestFit
	{
		get
		{
			return bestFitField;
		}
		set
		{
			bestFitField = value;
		}
	}

	[DefaultValue(false)]
	[XmlAttribute]
	public bool customWidth
	{
		get
		{
			return customWidthField;
		}
		set
		{
			customWidthField = value;
		}
	}

	[DefaultValue(false)]
	[XmlAttribute]
	public bool phonetic
	{
		get
		{
			return phoneticField;
		}
		set
		{
			phoneticField = value;
		}
	}

	[DefaultValue(typeof(byte), "0")]
	[XmlAttribute]
	public byte outlineLevel
	{
		get
		{
			return outlineLevelField;
		}
		set
		{
			outlineLevelField = value;
		}
	}

	[DefaultValue(true)]
	[XmlAttribute]
	public bool collapsed
	{
		get
		{
			return collapsedField;
		}
		set
		{
			collapsedField = value;
			collapsedSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool collapsedSpecified
	{
		get
		{
			return collapsedSpecifiedField;
		}
		set
		{
			collapsedSpecifiedField = value;
		}
	}

	public bool IsSetBestFit()
	{
		return bestFitField;
	}

	public bool IsSetCustomWidth()
	{
		return customWidthField;
	}

	public bool IsSetHidden()
	{
		return hiddenField;
	}

	public bool IsSetStyle()
	{
		return styleField.HasValue;
	}

	public bool IsSetWidth()
	{
		return widthField > 0.0;
	}

	public bool IsSetCollapsed()
	{
		return collapsedSpecifiedField;
	}

	public bool IsSetPhonetic()
	{
		return phoneticField;
	}

	public bool IsSetOutlineLevel()
	{
		return outlineLevelField != 0;
	}

	public void UnsetHidden()
	{
		hiddenField = false;
	}

	public void UnsetCollapsed()
	{
		collapsedField = true;
		collapsedSpecified = false;
	}

	public static CT_Col Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Col cT_Col = new CT_Col();
		cT_Col.min = XmlHelper.ReadUInt(node.Attributes["min"]);
		cT_Col.max = XmlHelper.ReadUInt(node.Attributes["max"]);
		cT_Col.width = XmlHelper.ReadDouble(node.Attributes["width"]);
		if (node.Attributes["style"] != null)
		{
			cT_Col.style = XmlHelper.ReadUInt(node.Attributes["style"]);
		}
		else
		{
			cT_Col.style = null;
		}
		cT_Col.hidden = XmlHelper.ReadBool(node.Attributes["hidden"]);
		cT_Col.bestFit = XmlHelper.ReadBool(node.Attributes["bestFit"]);
		cT_Col.outlineLevel = XmlHelper.ReadByte(node.Attributes["outlineLevel"]);
		cT_Col.customWidth = XmlHelper.ReadBool(node.Attributes["customWidth"]);
		cT_Col.phonetic = XmlHelper.ReadBool(node.Attributes["phonetic"]);
		cT_Col.collapsed = XmlHelper.ReadBool(node.Attributes["collapsed"]);
		return cT_Col;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "min", min);
		XmlHelper.WriteAttribute(sw, "max", max);
		XmlHelper.WriteAttribute(sw, "width", width);
		if (style.HasValue)
		{
			XmlHelper.WriteAttribute(sw, "style", style.Value, writeIfBlank: true);
		}
		XmlHelper.WriteAttribute(sw, "hidden", hidden, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "bestFit", bestFit, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "customWidth", customWidth, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "phonetic", phonetic, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "outlineLevel", outlineLevel);
		XmlHelper.WriteAttribute(sw, "collapsed", collapsed, writeIfBlank: false);
		sw.Write("/>");
	}

	public CT_Col Copy()
	{
		return new CT_Col
		{
			bestFitField = bestFitField,
			collapsedField = collapsedField,
			collapsedSpecifiedField = collapsedSpecifiedField,
			customWidthField = customWidthField,
			hiddenField = hiddenField,
			maxField = maxField,
			minField = minField,
			outlineLevelField = outlineLevelField,
			phoneticField = phoneticField,
			styleField = styleField,
			widthField = widthField,
			widthSpecifiedField = widthSpecifiedField
		};
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is CT_Col))
		{
			return false;
		}
		CT_Col cT_Col = obj as CT_Col;
		if (cT_Col.min == min)
		{
			return cT_Col.max == max;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override string ToString()
	{
		return $"min:{min}, max:{max}, width:{width}";
	}
}
