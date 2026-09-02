using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_Extension
{
	private string anyField;

	private string uriField;

	[XmlText]
	public string Any
	{
		get
		{
			return anyField;
		}
		set
		{
			anyField = value;
		}
	}

	[XmlAttribute(DataType = "token")]
	public string uri
	{
		get
		{
			return uriField;
		}
		set
		{
			uriField = value;
		}
	}

	public static CT_Extension Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Extension
		{
			uri = XmlHelper.ReadString(node.Attributes["uri"]),
			Any = node.InnerXml
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "uri", uri);
		sw.Write(">");
		if (Any != null)
		{
			sw.Write(Any);
		}
		sw.Write($"</c:{nodeName}>");
	}
}
