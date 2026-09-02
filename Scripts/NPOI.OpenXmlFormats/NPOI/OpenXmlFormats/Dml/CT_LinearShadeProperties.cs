using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_LinearShadeProperties
{
	private int angField;

	private bool angFieldSpecified;

	private bool scaledField;

	private bool scaledFieldSpecified;

	[XmlAttribute]
	public int ang
	{
		get
		{
			return angField;
		}
		set
		{
			angField = value;
		}
	}

	[XmlIgnore]
	public bool angSpecified
	{
		get
		{
			return angFieldSpecified;
		}
		set
		{
			angFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool scaled
	{
		get
		{
			return scaledField;
		}
		set
		{
			scaledField = value;
		}
	}

	[XmlIgnore]
	public bool scaledSpecified
	{
		get
		{
			return scaledFieldSpecified;
		}
		set
		{
			scaledFieldSpecified = value;
		}
	}

	public static CT_LinearShadeProperties Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_LinearShadeProperties
		{
			ang = XmlHelper.ReadInt(node.Attributes["ang"]),
			scaled = XmlHelper.ReadBool(node.Attributes["scaled"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "ang", ang);
		XmlHelper.WriteAttribute(sw, "scaled", scaled);
		sw.Write("/>");
	}
}
