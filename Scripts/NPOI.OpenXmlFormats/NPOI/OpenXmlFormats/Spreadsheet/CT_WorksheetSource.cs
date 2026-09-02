using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_WorksheetSource
{
	private string refField;

	private string nameField;

	private string sheetField;

	private string idField;

	[XmlAttribute]
	public string @ref
	{
		get
		{
			return refField;
		}
		set
		{
			refField = value;
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
	public string sheet
	{
		get
		{
			return sheetField;
		}
		set
		{
			sheetField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships")]
	public string id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	public static CT_WorksheetSource Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_WorksheetSource
		{
			@ref = XmlHelper.ReadString(node.Attributes["ref"]),
			name = XmlHelper.ReadString(node.Attributes["name"]),
			sheet = XmlHelper.ReadString(node.Attributes["sheet"]),
			id = XmlHelper.ReadString(node.Attributes["r:id"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "ref", @ref);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "sheet", sheet);
		XmlHelper.WriteAttribute(sw, "r:id", id);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
