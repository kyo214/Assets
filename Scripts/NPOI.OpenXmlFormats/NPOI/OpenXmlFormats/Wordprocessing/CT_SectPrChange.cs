using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_SectPrChange : CT_TrackChange
{
	private CT_SectPrBase sectPrField;

	[XmlElement(Order = 0)]
	public CT_SectPrBase sectPr
	{
		get
		{
			return sectPrField;
		}
		set
		{
			sectPrField = value;
		}
	}

	public CT_SectPrChange()
	{
		sectPrField = new CT_SectPrBase();
	}

	public new static CT_SectPrChange Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_SectPrChange cT_SectPrChange = new CT_SectPrChange();
		cT_SectPrChange.author = XmlHelper.ReadString(node.Attributes["w:author"]);
		cT_SectPrChange.date = XmlHelper.ReadString(node.Attributes["w:date"]);
		cT_SectPrChange.id = XmlHelper.ReadString(node.Attributes["r:id"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "sectPr")
			{
				cT_SectPrChange.sectPr = CT_SectPrBase.Parse(childNode, namespaceManager);
			}
		}
		return cT_SectPrChange;
	}

	internal new void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:author", base.author);
		XmlHelper.WriteAttribute(sw, "w:date", base.date);
		XmlHelper.WriteAttribute(sw, "r:id", base.id);
		sw.Write(">");
		if (sectPr != null)
		{
			sectPr.Write(sw, "sectPr");
		}
		sw.Write($"</w:{nodeName}>");
	}
}
