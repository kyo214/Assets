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
public class CT_StrVal
{
	private string vField;

	private uint idxField;

	[XmlElement(Order = 0)]
	public string v
	{
		get
		{
			return vField;
		}
		set
		{
			vField = value;
		}
	}

	[XmlAttribute]
	public uint idx
	{
		get
		{
			return idxField;
		}
		set
		{
			idxField = value;
		}
	}

	public static CT_StrVal Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_StrVal cT_StrVal = new CT_StrVal();
		cT_StrVal.idx = XmlHelper.ReadUInt(node.Attributes["idx"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "v")
			{
				cT_StrVal.v = childNode.InnerText;
			}
		}
		return cT_StrVal;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "idx", idx, writeIfBlank: true);
		sw.Write(">");
		if (v != null)
		{
			sw.Write($"<c:v>{v}</c:v>");
		}
		sw.Write($"</c:{nodeName}>");
	}
}
