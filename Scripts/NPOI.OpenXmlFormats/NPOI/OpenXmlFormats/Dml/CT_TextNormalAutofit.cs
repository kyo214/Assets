using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_TextNormalAutofit
{
	private int fontScaleField;

	private int lnSpcReductionField;

	[XmlAttribute]
	[DefaultValue(100000)]
	public int fontScale
	{
		get
		{
			return fontScaleField;
		}
		set
		{
			fontScaleField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int lnSpcReduction
	{
		get
		{
			return lnSpcReductionField;
		}
		set
		{
			lnSpcReductionField = value;
		}
	}

	public CT_TextNormalAutofit()
	{
		fontScaleField = 100000;
		lnSpcReductionField = 0;
	}

	public static CT_TextNormalAutofit Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TextNormalAutofit
		{
			fontScale = XmlHelper.ReadInt(node.Attributes["fontScale"]),
			lnSpcReduction = XmlHelper.ReadInt(node.Attributes["lnSpcReduction"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "fontScale", fontScale);
		XmlHelper.WriteAttribute(sw, "lnSpcReduction", lnSpcReduction);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
