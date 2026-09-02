using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing", IsNullable = true)]
public class CT_PosH
{
	private ST_RelFromH relativeFromField;

	private int? posOffsetField;

	private ST_AlignH? alignField;

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

	public ST_AlignH? align
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
	public ST_RelFromH relativeFrom
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

	public static CT_PosH Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PosH cT_PosH = new CT_PosH();
		if (node.Attributes["relativeFrom"] != null)
		{
			cT_PosH.relativeFrom = (ST_RelFromH)Enum.Parse(typeof(ST_RelFromH), node.Attributes["relativeFrom"].Value);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "posOffset")
			{
				cT_PosH.posOffset = (int.TryParse(childNode.InnerText, out var result) ? new int?(result) : ((int?)null));
			}
			else if (childNode.LocalName == "align")
			{
				cT_PosH.align = (ST_AlignH)Enum.Parse(typeof(ST_AlignH), childNode.InnerText);
			}
		}
		return cT_PosH;
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
