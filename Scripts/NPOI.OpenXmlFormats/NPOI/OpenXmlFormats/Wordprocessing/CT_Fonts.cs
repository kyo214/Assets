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
public class CT_Fonts
{
	private ST_Hint hintField;

	private bool hintFieldSpecified;

	private string asciiField;

	private string hAnsiField;

	private string eastAsiaField;

	private string csField;

	private ST_Theme? asciiThemeField;

	private ST_Theme? hAnsiThemeField;

	private ST_Theme? eastAsiaThemeField;

	private ST_Theme? csthemeField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_Hint hint
	{
		get
		{
			return hintField;
		}
		set
		{
			hintField = value;
			hintFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool hintSpecified
	{
		get
		{
			return hintFieldSpecified;
		}
		set
		{
			hintFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string ascii
	{
		get
		{
			return asciiField;
		}
		set
		{
			asciiField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string hAnsi
	{
		get
		{
			return hAnsiField;
		}
		set
		{
			hAnsiField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string eastAsia
	{
		get
		{
			return eastAsiaField;
		}
		set
		{
			eastAsiaField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string cs
	{
		get
		{
			return csField;
		}
		set
		{
			csField = value;
		}
	}

	public ST_Theme? asciiTheme
	{
		get
		{
			return asciiThemeField;
		}
		set
		{
			asciiThemeField = value;
		}
	}

	public ST_Theme? hAnsiTheme
	{
		get
		{
			return hAnsiThemeField;
		}
		set
		{
			hAnsiThemeField = value;
		}
	}

	public ST_Theme? eastAsiaTheme
	{
		get
		{
			return eastAsiaThemeField;
		}
		set
		{
			eastAsiaThemeField = value;
		}
	}

	public ST_Theme? cstheme
	{
		get
		{
			return csthemeField;
		}
		set
		{
			csthemeField = value;
		}
	}

	public CT_Fonts()
	{
		hintField = ST_Hint.@default;
	}

	public static CT_Fonts Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Fonts cT_Fonts = new CT_Fonts();
		cT_Fonts.ascii = XmlHelper.ReadString(node.Attributes["w:ascii"]);
		cT_Fonts.hAnsi = XmlHelper.ReadString(node.Attributes["w:hAnsi"]);
		cT_Fonts.eastAsia = XmlHelper.ReadString(node.Attributes["w:eastAsia"]);
		cT_Fonts.cs = XmlHelper.ReadString(node.Attributes["w:cs"]);
		if (node.Attributes["w:hint"] != null)
		{
			cT_Fonts.hint = (ST_Hint)Enum.Parse(typeof(ST_Hint), node.Attributes["w:hint"].Value);
			cT_Fonts.hintFieldSpecified = true;
		}
		if (node.Attributes["w:asciiTheme"] != null)
		{
			cT_Fonts.asciiTheme = (ST_Theme)Enum.Parse(typeof(ST_Theme), node.Attributes["w:asciiTheme"].Value);
		}
		if (node.Attributes["w:hAnsiTheme"] != null)
		{
			cT_Fonts.hAnsiTheme = (ST_Theme)Enum.Parse(typeof(ST_Theme), node.Attributes["w:hAnsiTheme"].Value);
		}
		if (node.Attributes["w:eastAsiaTheme"] != null)
		{
			cT_Fonts.eastAsiaTheme = (ST_Theme)Enum.Parse(typeof(ST_Theme), node.Attributes["w:eastAsiaTheme"].Value);
		}
		if (node.Attributes["w:cstheme"] != null)
		{
			cT_Fonts.cstheme = (ST_Theme)Enum.Parse(typeof(ST_Theme), node.Attributes["w:cstheme"].Value);
		}
		return cT_Fonts;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		if (hint != ST_Hint.@default || hintFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "w:hint", hint.ToString());
		}
		XmlHelper.WriteAttribute(sw, "w:ascii", ascii);
		XmlHelper.WriteAttribute(sw, "w:hAnsi", hAnsi);
		XmlHelper.WriteAttribute(sw, "w:eastAsia", eastAsia);
		XmlHelper.WriteAttribute(sw, "w:cs", cs);
		if (asciiTheme.HasValue)
		{
			XmlHelper.WriteAttribute(sw, "w:asciiTheme", asciiTheme.ToString());
		}
		if (eastAsiaTheme.HasValue)
		{
			XmlHelper.WriteAttribute(sw, "w:eastAsiaTheme", eastAsiaTheme.ToString());
		}
		if (hAnsiTheme != ST_Theme.majorEastAsia)
		{
			XmlHelper.WriteAttribute(sw, "w:hAnsiTheme", hAnsiTheme.ToString());
		}
		if (cstheme.HasValue)
		{
			XmlHelper.WriteAttribute(sw, "w:cstheme", cstheme.ToString());
		}
		sw.Write("/>");
	}

	public bool IsSetHAnsi()
	{
		return !string.IsNullOrEmpty(hAnsiField);
	}

	public bool IsSetCs()
	{
		return !string.IsNullOrEmpty(csField);
	}

	public bool IsSetEastAsia()
	{
		return !string.IsNullOrEmpty(eastAsiaField);
	}
}
