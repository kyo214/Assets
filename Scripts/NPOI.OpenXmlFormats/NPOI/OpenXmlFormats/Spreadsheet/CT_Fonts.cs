using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(ElementName = "fonts", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public class CT_Fonts
{
	private List<CT_Font> fontField;

	private uint countField;

	private bool countFieldSpecified;

	private uint knownFontsField;

	[XmlElement]
	public List<CT_Font> font
	{
		get
		{
			return fontField;
		}
		set
		{
			fontField = value;
		}
	}

	[XmlAttribute]
	public uint count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
		}
	}

	[XmlIgnore]
	public bool countSpecified
	{
		get
		{
			return countFieldSpecified;
		}
		set
		{
			countFieldSpecified = value;
		}
	}

	public static CT_Fonts Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Fonts cT_Fonts = new CT_Fonts();
		cT_Fonts.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		cT_Fonts.knownFontsField = XmlHelper.ReadUInt(node.Attributes["x14ac:knownFonts"]);
		cT_Fonts.font = new List<CT_Font>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "font")
			{
				cT_Fonts.font.Add(CT_Font.Parse(childNode, namespaceManager));
			}
		}
		return cT_Fonts;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		XmlHelper.WriteAttribute(sw, "x14ac:knownFonts", knownFontsField);
		sw.Write(">");
		if (font != null)
		{
			foreach (CT_Font item in font)
			{
				item.Write(sw, "font");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public void SetFontArray(List<CT_Font> array)
	{
		fontField = array;
	}
}
