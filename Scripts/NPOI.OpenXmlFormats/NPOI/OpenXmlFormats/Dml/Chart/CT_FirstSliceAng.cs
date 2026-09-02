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
public class CT_FirstSliceAng
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

	public CT_FirstSliceAng()
	{
		valField = 0;
	}

	public static CT_FirstSliceAng Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FirstSliceAng cT_FirstSliceAng = new CT_FirstSliceAng();
		if (node.Attributes["val"] != null)
		{
			cT_FirstSliceAng.val = XmlHelper.ReadUShort(node.Attributes["val"]);
		}
		return cT_FirstSliceAng;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val);
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
