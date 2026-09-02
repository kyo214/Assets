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
public class CT_LblOffset
{
	private ushort valField;

	[XmlAttribute]
	[DefaultValue(typeof(ushort), "100")]
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

	public CT_LblOffset()
	{
		valField = 100;
	}

	public static CT_LblOffset Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_LblOffset cT_LblOffset = new CT_LblOffset();
		if (node.Attributes["val"] != null)
		{
			cT_LblOffset.val = XmlHelper.ReadUShort(node.Attributes["val"]);
		}
		return cT_LblOffset;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val);
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
