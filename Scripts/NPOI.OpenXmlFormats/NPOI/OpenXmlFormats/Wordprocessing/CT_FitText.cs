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
public class CT_FitText
{
	private ulong valField;

	private string idField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "integer")]
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

	public static CT_FitText Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_FitText
		{
			val = XmlHelper.ReadULong(node.Attributes["w:val"]),
			id = XmlHelper.ReadString(node.Attributes["w:id"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val);
		XmlHelper.WriteAttribute(sw, "w:id", id);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
