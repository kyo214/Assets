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
public class CT_GeomGuide
{
	private string nameField;

	private string fmlaField;

	[XmlAttribute(DataType = "token")]
	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	[XmlAttribute]
	public string fmla
	{
		get
		{
			return fmlaField;
		}
		set
		{
			fmlaField = value;
		}
	}

	public static CT_GeomGuide Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_GeomGuide
		{
			name = XmlHelper.ReadString(node.Attributes["name"]),
			fmla = XmlHelper.ReadString(node.Attributes["fmla"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "fmla", fmla);
		sw.Write("/>");
	}
}
