using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_SupplementalFont
{
	private string scriptField;

	private string typefaceField;

	[XmlAttribute]
	public string script
	{
		get
		{
			return scriptField;
		}
		set
		{
			scriptField = value;
		}
	}

	[XmlAttribute]
	public string typeface
	{
		get
		{
			return typefaceField;
		}
		set
		{
			typefaceField = value;
		}
	}

	public static CT_SupplementalFont Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_SupplementalFont
		{
			script = XmlHelper.ReadString(node.Attributes["script"]),
			typeface = XmlHelper.ReadString(node.Attributes["typeface"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "script", script);
		XmlHelper.WriteAttribute(sw, "typeface", typeface, writeIfBlank: true);
		sw.Write("/>");
	}
}
