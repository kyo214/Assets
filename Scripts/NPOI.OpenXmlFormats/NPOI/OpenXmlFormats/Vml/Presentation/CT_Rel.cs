using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Vml.Presentation;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:powerpoint")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:powerpoint", IsNullable = true)]
public class CT_Rel
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

	public static CT_Rel Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Rel
		{
			id = XmlHelper.ReadString(node.Attributes["r:id"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<p:{nodeName}");
		XmlHelper.WriteAttribute(sw, "r:id", id);
		sw.Write(">");
		sw.Write($"</p:{nodeName}>");
	}
}
