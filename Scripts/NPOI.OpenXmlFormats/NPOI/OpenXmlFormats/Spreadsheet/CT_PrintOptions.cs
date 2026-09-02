using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_PrintOptions
{
	private bool horizontalCenteredField;

	private bool verticalCenteredField;

	private bool headingsField;

	private bool gridLinesField;

	private bool gridLinesSetField;

	[XmlAttribute]
	[DefaultValue(false)]
	public bool horizontalCentered
	{
		get
		{
			return horizontalCenteredField;
		}
		set
		{
			horizontalCenteredField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool verticalCentered
	{
		get
		{
			return verticalCenteredField;
		}
		set
		{
			verticalCenteredField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool headings
	{
		get
		{
			return headingsField;
		}
		set
		{
			headingsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool gridLines
	{
		get
		{
			return gridLinesField;
		}
		set
		{
			gridLinesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool gridLinesSet
	{
		get
		{
			return gridLinesSetField;
		}
		set
		{
			gridLinesSetField = value;
		}
	}

	public static CT_PrintOptions Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_PrintOptions
		{
			horizontalCentered = XmlHelper.ReadBool(node.Attributes["horizontalCentered"]),
			verticalCentered = XmlHelper.ReadBool(node.Attributes["verticalCentered"]),
			headings = XmlHelper.ReadBool(node.Attributes["headings"]),
			gridLines = XmlHelper.ReadBool(node.Attributes["gridLines"]),
			gridLinesSet = XmlHelper.ReadBool(node.Attributes["gridLinesSet"], blankValue: true)
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		if (horizontalCentered)
		{
			XmlHelper.WriteAttribute(sw, "horizontalCentered", horizontalCentered);
		}
		if (verticalCentered)
		{
			XmlHelper.WriteAttribute(sw, "verticalCentered", verticalCentered);
		}
		XmlHelper.WriteAttribute(sw, "headings", headings, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "gridLines", gridLines, writeIfBlank: false);
		if (!gridLinesSet)
		{
			XmlHelper.WriteAttribute(sw, "gridLinesSet", gridLinesSet);
		}
		sw.Write("/>");
	}

	public CT_PrintOptions()
	{
		horizontalCenteredField = false;
		verticalCenteredField = false;
		headingsField = false;
		gridLinesField = false;
		gridLinesSetField = true;
	}
}
