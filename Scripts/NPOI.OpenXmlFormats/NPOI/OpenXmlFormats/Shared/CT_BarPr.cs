using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_BarPr
{
	private CT_TopBot posField;

	private CT_CtrlPr ctrlPrField;

	[XmlElement(Order = 0)]
	public CT_TopBot pos
	{
		get
		{
			return posField;
		}
		set
		{
			posField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_CtrlPr ctrlPr
	{
		get
		{
			return ctrlPrField;
		}
		set
		{
			ctrlPrField = value;
		}
	}

	public static CT_BarPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_BarPr cT_BarPr = new CT_BarPr();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pos")
			{
				cT_BarPr.pos = CT_TopBot.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "ctrlPr")
			{
				cT_BarPr.ctrlPr = CT_CtrlPr.Parse(childNode, namespaceManager);
			}
		}
		return cT_BarPr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		sw.Write(">");
		if (pos != null)
		{
			pos.Write(sw, "pos");
		}
		if (ctrlPr != null)
		{
			ctrlPr.Write(sw, "ctrlPr");
		}
		sw.Write($"</m:{nodeName}>");
	}
}
