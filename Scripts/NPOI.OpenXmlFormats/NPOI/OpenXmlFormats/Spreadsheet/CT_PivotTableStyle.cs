using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotTableStyle
{
	private string nameField;

	private bool showRowHeadersField;

	private bool showRowHeadersFieldSpecified;

	private bool showColHeadersField;

	private bool showColHeadersFieldSpecified;

	private bool showRowStripesField;

	private bool showRowStripesFieldSpecified;

	private bool showColStripesField;

	private bool showColStripesFieldSpecified;

	private bool showLastColumnField;

	private bool showLastColumnFieldSpecified;

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
	public bool showRowHeaders
	{
		get
		{
			return showRowHeadersField;
		}
		set
		{
			showRowHeadersField = value;
		}
	}

	[XmlIgnore]
	public bool showRowHeadersSpecified
	{
		get
		{
			return showRowHeadersFieldSpecified;
		}
		set
		{
			showRowHeadersFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool showColHeaders
	{
		get
		{
			return showColHeadersField;
		}
		set
		{
			showColHeadersField = value;
		}
	}

	[XmlIgnore]
	public bool showColHeadersSpecified
	{
		get
		{
			return showColHeadersFieldSpecified;
		}
		set
		{
			showColHeadersFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool showRowStripes
	{
		get
		{
			return showRowStripesField;
		}
		set
		{
			showRowStripesField = value;
		}
	}

	[XmlIgnore]
	public bool showRowStripesSpecified
	{
		get
		{
			return showRowStripesFieldSpecified;
		}
		set
		{
			showRowStripesFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool showColStripes
	{
		get
		{
			return showColStripesField;
		}
		set
		{
			showColStripesField = value;
		}
	}

	[XmlIgnore]
	public bool showColStripesSpecified
	{
		get
		{
			return showColStripesFieldSpecified;
		}
		set
		{
			showColStripesFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool showLastColumn
	{
		get
		{
			return showLastColumnField;
		}
		set
		{
			showLastColumnField = value;
		}
	}

	[XmlIgnore]
	public bool showLastColumnSpecified
	{
		get
		{
			return showLastColumnFieldSpecified;
		}
		set
		{
			showLastColumnFieldSpecified = value;
		}
	}

	public static CT_PivotTableStyle Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotTableStyle cT_PivotTableStyle = new CT_PivotTableStyle();
		cT_PivotTableStyle.name = XmlHelper.ReadString(node.Attributes["name"]);
		if (node.Attributes["showRowHeaders"] != null)
		{
			cT_PivotTableStyle.showRowHeaders = XmlHelper.ReadBool(node.Attributes["showRowHeaders"]);
		}
		if (node.Attributes["showColHeaders"] != null)
		{
			cT_PivotTableStyle.showColHeaders = XmlHelper.ReadBool(node.Attributes["showColHeaders"]);
		}
		if (node.Attributes["showRowStripes"] != null)
		{
			cT_PivotTableStyle.showRowStripes = XmlHelper.ReadBool(node.Attributes["showRowStripes"]);
		}
		if (node.Attributes["showColStripes"] != null)
		{
			cT_PivotTableStyle.showColStripes = XmlHelper.ReadBool(node.Attributes["showColStripes"]);
		}
		if (node.Attributes["showLastColumn"] != null)
		{
			cT_PivotTableStyle.showLastColumn = XmlHelper.ReadBool(node.Attributes["showLastColumn"]);
		}
		return cT_PivotTableStyle;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "showRowHeaders", showRowHeaders);
		XmlHelper.WriteAttribute(sw, "showColHeaders", showColHeaders);
		XmlHelper.WriteAttribute(sw, "showRowStripes", showRowStripes);
		XmlHelper.WriteAttribute(sw, "showColStripes", showColStripes);
		XmlHelper.WriteAttribute(sw, "showLastColumn", showLastColumn);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
