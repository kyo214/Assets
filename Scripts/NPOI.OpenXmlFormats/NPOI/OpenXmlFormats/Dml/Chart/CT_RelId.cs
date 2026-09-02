using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot("chart", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = false)]
public class CT_RelId
{
	private string idField;

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

	public static CT_RelId Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_RelId
		{
			id = XmlHelper.ReadString(node.Attributes["r:id"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "r:id", id);
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
