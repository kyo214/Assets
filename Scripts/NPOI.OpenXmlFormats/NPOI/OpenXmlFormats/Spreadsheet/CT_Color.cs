using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot("color", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Color
{
	private bool autoField;

	private uint indexedField;

	private byte[] rgbField;

	private uint themeField;

	private double tintField;

	private bool autoSpecifiedField;

	private bool indexedSpecifiedField;

	private bool rgbSpecifiedField;

	private bool themeSpecifiedField;

	private bool tintSpecifiedField;

	[XmlAttribute]
	public bool auto
	{
		get
		{
			return autoField;
		}
		set
		{
			autoField = value;
			autoSpecified = true;
		}
	}

	[XmlIgnore]
	public bool autoSpecified
	{
		get
		{
			return autoSpecifiedField;
		}
		set
		{
			autoSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint indexed
	{
		get
		{
			return indexedField;
		}
		set
		{
			indexedField = value;
			indexedSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool indexedSpecified
	{
		get
		{
			return indexedSpecifiedField;
		}
		set
		{
			indexedSpecifiedField = value;
		}
	}

	[XmlAttribute(DataType = "hexBinary")]
	public byte[] rgb
	{
		get
		{
			return rgbField;
		}
		set
		{
			rgbField = value;
			rgbSpecified = true;
		}
	}

	[XmlIgnore]
	public bool rgbSpecified
	{
		get
		{
			return rgbSpecifiedField;
		}
		set
		{
			rgbSpecifiedField = value;
		}
	}

	[XmlAttribute]
	public uint theme
	{
		get
		{
			return themeField;
		}
		set
		{
			themeField = value;
			themeSpecifiedField = true;
		}
	}

	[XmlIgnore]
	public bool themeSpecified
	{
		get
		{
			return themeSpecifiedField;
		}
		set
		{
			themeSpecifiedField = value;
		}
	}

	[DefaultValue(0.0)]
	[XmlAttribute]
	public double tint
	{
		get
		{
			return tintField;
		}
		set
		{
			tintField = value;
			tintSpecified = true;
		}
	}

	[XmlIgnore]
	public bool tintSpecified
	{
		get
		{
			return tintSpecifiedField;
		}
		set
		{
			tintSpecifiedField = value;
		}
	}

	public bool IsSetAuto()
	{
		return autoSpecifiedField;
	}

	public bool IsSetIndexed()
	{
		return indexedSpecified;
	}

	public void SetRgb(byte R, byte G, byte B)
	{
		rgbField = new byte[3];
		rgbField[0] = R;
		rgbField[1] = G;
		rgbField[2] = B;
		rgbSpecified = true;
	}

	public bool IsSetRgb()
	{
		return rgbSpecified;
	}

	public void SetRgb(byte[] rgb)
	{
		rgbField = new byte[rgb.Length];
		Array.Copy(rgb, rgbField, rgb.Length);
		rgbSpecified = true;
	}

	public byte[] GetRgb()
	{
		if (rgbField == null)
		{
			return null;
		}
		byte[] array = new byte[rgbField.Length];
		Array.Copy(rgbField, array, rgbField.Length);
		return array;
	}

	public bool IsSetTheme()
	{
		return themeSpecified;
	}

	public bool IsSetTint()
	{
		return tintSpecified;
	}

	public static CT_Color Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Color
		{
			auto = XmlHelper.ReadBool(node.Attributes["auto"]),
			autoSpecified = (node.Attributes["auto"] != null),
			indexed = XmlHelper.ReadUInt(node.Attributes["indexed"]),
			indexedSpecified = (node.Attributes["indexed"] != null),
			rgb = XmlHelper.ReadBytes(node.Attributes["rgb"]),
			rgbSpecified = (node.Attributes["rgb"] != null),
			theme = XmlHelper.ReadUInt(node.Attributes["theme"]),
			themeSpecified = (node.Attributes["theme"] != null),
			tint = XmlHelper.ReadDouble(node.Attributes["tint"]),
			tintSpecified = (node.Attributes["tint"] != null)
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "auto", auto, writeIfBlank: false);
		if (indexedSpecified)
		{
			XmlHelper.WriteAttribute(sw, "indexed", indexed, writeIfBlank: true);
		}
		if (rgbSpecified)
		{
			XmlHelper.WriteAttribute(sw, "rgb", rgb);
		}
		if (themeSpecified)
		{
			XmlHelper.WriteAttribute(sw, "theme", theme, writeIfBlank: true);
		}
		if (tintSpecified)
		{
			XmlHelper.WriteAttribute(sw, "tint", tint);
		}
		sw.Write("/>");
	}

	public CT_Color Copy()
	{
		return new CT_Color
		{
			autoField = autoField,
			indexedField = indexedField,
			indexedSpecified = indexedSpecified,
			rgbField = ((rgbField == null) ? null : ((byte[])rgbField.Clone())),
			rgbSpecified = rgbSpecified,
			themeField = themeField,
			themeSpecified = themeSpecified,
			tintField = tintField,
			tintSpecified = tintSpecified
		};
	}

	public void UnsetIndexed()
	{
		indexedField = 0u;
		indexedSpecifiedField = false;
	}
}
