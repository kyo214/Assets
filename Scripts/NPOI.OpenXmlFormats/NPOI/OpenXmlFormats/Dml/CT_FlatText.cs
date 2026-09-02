using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_FlatText
{
	private long zField;

	[XmlAttribute]
	[DefaultValue(typeof(long), "0")]
	public long z
	{
		get
		{
			return zField;
		}
		set
		{
			zField = value;
		}
	}

	public CT_FlatText()
	{
		zField = 0L;
	}

	public static CT_FlatText Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_FlatText
		{
			z = XmlHelper.ReadLong(node.Attributes["z"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "z", z);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
