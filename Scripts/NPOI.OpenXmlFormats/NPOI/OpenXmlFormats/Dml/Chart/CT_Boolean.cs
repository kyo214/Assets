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
public class CT_Boolean
{
	private bool valField;

	[XmlAttribute]
	public int val
	{
		get
		{
			if (!valField)
			{
				return 0;
			}
			return 1;
		}
		set
		{
			valField = value == 1;
		}
	}

	public CT_Boolean()
	{
		valField = true;
	}

	public static CT_Boolean Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Boolean
		{
			val = XmlHelper.ReadInt(node.Attributes["val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val, writeIfBlank: true);
		sw.Write("/>");
	}
}
