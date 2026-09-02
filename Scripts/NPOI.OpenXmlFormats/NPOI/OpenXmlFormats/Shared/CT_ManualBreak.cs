using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_ManualBreak
{
	private string alnAtField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "integer")]
	public string alnAt
	{
		get
		{
			return alnAtField;
		}
		set
		{
			alnAtField = value;
		}
	}

	public static CT_ManualBreak Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_ManualBreak
		{
			alnAt = XmlHelper.ReadString(node.Attributes["m:alnAt"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:alnAt", alnAt);
		sw.Write(">");
		sw.Write($"</m:{nodeName}>");
	}
}
