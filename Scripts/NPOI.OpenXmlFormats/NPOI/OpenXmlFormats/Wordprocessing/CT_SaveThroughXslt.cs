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
public class CT_SaveThroughXslt
{
	private string idField;

	private string solutionIDField;

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

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string solutionID
	{
		get
		{
			return solutionIDField;
		}
		set
		{
			solutionIDField = value;
		}
	}

	public static CT_SaveThroughXslt Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_SaveThroughXslt
		{
			id = XmlHelper.ReadString(node.Attributes["r:id"]),
			solutionID = XmlHelper.ReadString(node.Attributes["w:solutionID"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "r:id", id);
		XmlHelper.WriteAttribute(sw, "w:solutionID", solutionID);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
