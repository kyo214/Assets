using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_OfficeArtExtension
{
	private string anyField;

	private string uriField;

	private bool uriSpecifiedField;

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

	public bool uriSpecified
	{
		get
		{
			return uriSpecifiedField;
		}
		set
		{
			uriSpecifiedField = value;
		}
	}

	public static CT_OfficeArtExtension Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_OfficeArtExtension
		{
			uri = XmlHelper.ReadString(node.Attributes["uri"]),
			Any = node.InnerXml.Replace(" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"", "")
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "uri", uri);
		sw.Write(">");
		if (!string.IsNullOrEmpty(anyField))
		{
			sw.Write(anyField);
		}
		sw.Write($"</a:{nodeName}>");
	}
}
