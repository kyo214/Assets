using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing", IsNullable = true)]
public class CT_PosV
{
	private ST_RelFromV relativeFromField;

	private int? posOffsetField;

	private ST_AlignV? alignField;

	public int? posOffset
	{
		get
		{
			return posOffsetField;
		}
		set
		{
			posOffsetField = value;
		}
	}

	public ST_AlignV? align
	{
		get
		{
			return alignField;
		}
		set
		{
			alignField = value;
		}
	}

	[XmlAttribute]
	public ST_RelFromV relativeFrom
	{
		get
		{
			return relativeFromField;
		}
		set
		{
			relativeFromField = value;
		}
	}

	public static CT_PosV Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PosV cT_PosV = new CT_PosV();
		if (node.Attributes["relativeFrom"] != null)
		{
			cT_PosV.relativeFrom = (ST_RelFromV)Enum.Parse(typeof(ST_RelFromV), node.Attributes["relativeFrom"].Value);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "posOffset")
			{
				cT_PosV.posOffset = (int.TryParse(childNode.InnerText, out var result) ? new int?(result) : ((int?)null));
			}
			else if (childNode.LocalName == "align")
			{
				cT_PosV.align = (ST_AlignV)Enum.Parse(typeof(ST_AlignV), childNode.InnerText);
			}
		}
		return cT_PosV;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<wp:{nodeName}");
		XmlHelper.WriteAttribute(sw, "relativeFrom", relativeFrom.ToString());
		sw.Write(">");
		if (posOffset.HasValue)
		{
			sw.Write($"<wp:posOffset>{posOffset.Value}</wp:posOffset>");
		}
		if (align.HasValue)
		{
			sw.Write($"<wp:align>{align.Value}</wp:align>");
		}
		sw.Write($"</wp:{nodeName}>");
	}
}
