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
public class CT_GapAmount
{
	private ushort valField;

	[XmlAttribute]
	[DefaultValue(typeof(ushort), "150")]
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

	public CT_GapAmount()
	{
		valField = 150;
	}

	public static CT_GapAmount Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GapAmount cT_GapAmount = new CT_GapAmount();
		if (node.Attributes["val"] != null)
		{
			cT_GapAmount.val = XmlHelper.ReadUShort(node.Attributes["val"]);
		}
		return cT_GapAmount;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val);
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
