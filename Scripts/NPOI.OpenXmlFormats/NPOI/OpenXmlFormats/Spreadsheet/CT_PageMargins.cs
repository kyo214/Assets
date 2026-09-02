using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_PageMargins
{
	private double leftField;

	private double rightField;

	private double topField;

	private double bottomField;

	private double headerField;

	private double footerField;

	[XmlAttribute]
	public double left
	{
		get
		{
			return leftField;
		}
		set
		{
			leftField = value;
		}
	}

	[XmlAttribute]
	public double right
	{
		get
		{
			return rightField;
		}
		set
		{
			rightField = value;
		}
	}

	[XmlAttribute]
	public double top
	{
		get
		{
			return topField;
		}
		set
		{
			topField = value;
		}
	}

	[XmlAttribute]
	public double bottom
	{
		get
		{
			return bottomField;
		}
		set
		{
			bottomField = value;
		}
	}

	[XmlAttribute]
	public double header
	{
		get
		{
			return headerField;
		}
		set
		{
			headerField = value;
		}
	}

	[XmlAttribute]
	public double footer
	{
		get
		{
			return footerField;
		}
		set
		{
			footerField = value;
		}
	}

	public static CT_PageMargins Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_PageMargins
		{
			left = XmlHelper.ReadDouble(node.Attributes["left"]),
			right = XmlHelper.ReadDouble(node.Attributes["right"]),
			top = XmlHelper.ReadDouble(node.Attributes["top"]),
			bottom = XmlHelper.ReadDouble(node.Attributes["bottom"]),
			header = XmlHelper.ReadDouble(node.Attributes["header"]),
			footer = XmlHelper.ReadDouble(node.Attributes["footer"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "left", left, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "right", right, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "top", top, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "bottom", bottom, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "header", header, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "footer", footer, writeIfBlank: true);
		sw.Write("/>");
	}
}
