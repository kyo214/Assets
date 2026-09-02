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
public class CT_RotY
{
	private ushort valField;

	[XmlAttribute]
	[DefaultValue(typeof(ushort), "0")]
	public ushort val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	public CT_RotY()
	{
		valField = 0;
	}

	public static CT_RotY Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_RotY
		{
			val = XmlHelper.ReadUShort(node.Attributes["val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val);
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
