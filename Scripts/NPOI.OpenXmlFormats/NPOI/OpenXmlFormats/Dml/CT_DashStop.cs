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
public class CT_DashStop
{
	private int dField;

	private int spField;

	[XmlAttribute]
	public int d
	{
		get
		{
			return dField;
		}
		set
		{
			dField = value;
		}
	}

	[XmlAttribute]
	public int sp
	{
		get
		{
			return spField;
		}
		set
		{
			spField = value;
		}
	}

	public static CT_DashStop Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_DashStop
		{
			d = XmlHelper.ReadInt(node.Attributes["d"]),
			sp = XmlHelper.ReadInt(node.Attributes["sp"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "d", d);
		XmlHelper.WriteAttribute(sw, "sp", sp);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
