using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotDimension
{
	private bool measureField;

	private string nameField;

	private string uniqueNameField;

	private string captionField;

	[XmlAttribute]
	[DefaultValue(false)]
	public bool measure
	{
		get
		{
			return measureField;
		}
		set
		{
			measureField = value;
		}
	}

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
	public string uniqueName
	{
		get
		{
			return uniqueNameField;
		}
		set
		{
			uniqueNameField = value;
		}
	}

	[XmlAttribute]
	public string caption
	{
		get
		{
			return captionField;
		}
		set
		{
			captionField = value;
		}
	}

	public static CT_PivotDimension Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotDimension cT_PivotDimension = new CT_PivotDimension();
		if (node.Attributes["measure"] != null)
		{
			cT_PivotDimension.measure = XmlHelper.ReadBool(node.Attributes["measure"]);
		}
		cT_PivotDimension.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_PivotDimension.uniqueName = XmlHelper.ReadString(node.Attributes["uniqueName"]);
		cT_PivotDimension.caption = XmlHelper.ReadString(node.Attributes["caption"]);
		return cT_PivotDimension;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "measure", measure);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "uniqueName", uniqueName);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotDimension()
	{
		measureField = false;
	}
}
