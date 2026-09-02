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
public class CT_TextBulletSizePercent
{
	private int valField;

	private bool valFieldSpecified;

	[XmlAttribute]
	public int val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
			valFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool valSpecified
	{
		get
		{
			return valFieldSpecified;
		}
		set
		{
			valFieldSpecified = value;
		}
	}

	public static CT_TextBulletSizePercent Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TextBulletSizePercent
		{
			val = XmlHelper.ReadInt(node.Attributes["val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
