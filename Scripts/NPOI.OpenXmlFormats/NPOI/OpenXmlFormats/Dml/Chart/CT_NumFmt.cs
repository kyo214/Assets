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
public class CT_NumFmt
{
	private string formatCodeField;

	private bool sourceLinkedField;

	[XmlAttribute]
	public string formatCode
	{
		get
		{
			return formatCodeField;
		}
		set
		{
			formatCodeField = value;
		}
	}

	[XmlAttribute]
	public bool sourceLinked
	{
		get
		{
			return sourceLinkedField;
		}
		set
		{
			sourceLinkedField = value;
		}
	}

	public static CT_NumFmt Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_NumFmt
		{
			formatCode = XmlHelper.ReadString(node.Attributes["formatCode"]),
			sourceLinked = XmlHelper.ReadBool(node.Attributes["sourceLinked"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "formatCode", formatCode);
		XmlHelper.WriteAttribute(sw, "sourceLinked", sourceLinked, writeIfBlank: false);
		sw.Write("/>");
	}
}
