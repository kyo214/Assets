using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
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

	[XmlIgnore]
	public bool uriSpecified => uriField != null;

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
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "uri", uri);
		sw.Write(">");
		if (Any != null)
		{
			sw.Write(Any);
		}
		sw.Write($"</{nodeName}>");
	}
}
