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
public class CT_LineJoinMiterProperties
{
	private int limField;

	private bool limFieldSpecified;

	[XmlAttribute]
	public int lim
	{
		get
		{
			return limField;
		}
		set
		{
			limField = value;
		}
	}

	[XmlIgnore]
	public bool limSpecified
	{
		get
		{
			return limFieldSpecified;
		}
		set
		{
			limFieldSpecified = value;
		}
	}

	public static CT_LineJoinMiterProperties Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_LineJoinMiterProperties
		{
			lim = XmlHelper.ReadInt(node.Attributes["lim"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "lim", lim);
		sw.Write("/>");
	}
}
