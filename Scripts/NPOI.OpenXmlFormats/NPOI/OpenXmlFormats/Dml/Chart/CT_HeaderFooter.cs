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
public class CT_HeaderFooter
{
	private string oddHeaderField;

	private string oddFooterField;

	private string evenHeaderField;

	private string evenFooterField;

	private string firstHeaderField;

	private string firstFooterField;

	private bool alignWithMarginsField;

	private bool differentOddEvenField;

	private bool differentFirstField;

	[XmlElement(Order = 0)]
	public string oddHeader
	{
		get
		{
			return oddHeaderField;
		}
		set
		{
			oddHeaderField = value;
		}
	}

	[XmlElement(Order = 1)]
	public string oddFooter
	{
		get
		{
			return oddFooterField;
		}
		set
		{
			oddFooterField = value;
		}
	}

	[XmlElement(Order = 2)]
	public string evenHeader
	{
		get
		{
			return evenHeaderField;
		}
		set
		{
			evenHeaderField = value;
		}
	}

	[XmlElement(Order = 3)]
	public string evenFooter
	{
		get
		{
			return evenFooterField;
		}
		set
		{
			evenFooterField = value;
		}
	}

	[XmlElement(Order = 4)]
	public string firstHeader
	{
		get
		{
			return firstHeaderField;
		}
		set
		{
			firstHeaderField = value;
		}
	}

	[XmlElement(Order = 5)]
	public string firstFooter
	{
		get
		{
			return firstFooterField;
		}
		set
		{
			firstFooterField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool alignWithMargins
	{
		get
		{
			return alignWithMarginsField;
		}
		set
		{
			alignWithMarginsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool differentOddEven
	{
		get
		{
			return differentOddEvenField;
		}
		set
		{
			differentOddEvenField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool differentFirst
	{
		get
		{
			return differentFirstField;
		}
		set
		{
			differentFirstField = value;
		}
	}

	public CT_HeaderFooter()
	{
		alignWithMarginsField = true;
		differentOddEvenField = false;
		differentFirstField = false;
	}

	public static CT_HeaderFooter Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_HeaderFooter cT_HeaderFooter = new CT_HeaderFooter();
		if (node.Attributes["alignWithMargins"] != null)
		{
			cT_HeaderFooter.alignWithMargins = XmlHelper.ReadBool(node.Attributes["alignWithMargins"]);
		}
		else
		{
			cT_HeaderFooter.alignWithMargins = true;
		}
		if (node.Attributes["differentOddEven"] != null)
		{
			cT_HeaderFooter.differentOddEven = XmlHelper.ReadBool(node.Attributes["differentOddEven"]);
		}
		if (node.Attributes["differentFirst"] != null)
		{
			cT_HeaderFooter.differentFirst = XmlHelper.ReadBool(node.Attributes["differentFirst"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "oddHeader")
			{
				cT_HeaderFooter.oddHeader = childNode.InnerText;
			}
			else if (childNode.LocalName == "oddFooter")
			{
				cT_HeaderFooter.oddFooter = childNode.InnerText;
			}
			else if (childNode.LocalName == "evenHeader")
			{
				cT_HeaderFooter.evenHeader = childNode.InnerText;
			}
			else if (childNode.LocalName == "evenFooter")
			{
				cT_HeaderFooter.evenFooter = childNode.InnerText;
			}
			else if (childNode.LocalName == "firstHeader")
			{
				cT_HeaderFooter.firstHeader = childNode.InnerText;
			}
			else if (childNode.LocalName == "firstFooter")
			{
				cT_HeaderFooter.firstFooter = childNode.InnerText;
			}
		}
		return cT_HeaderFooter;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		if (!alignWithMargins)
		{
			XmlHelper.WriteAttribute(sw, "alignWithMargins", alignWithMargins, writeIfBlank: true);
		}
		if (differentOddEven)
		{
			XmlHelper.WriteAttribute(sw, "differentOddEven", differentOddEven);
		}
		if (differentFirst)
		{
			XmlHelper.WriteAttribute(sw, "differentFirst", differentFirst);
		}
		sw.Write(">");
		if (oddHeader != null)
		{
			sw.Write($"<oddHeader>{oddHeader}</oddHeader>");
		}
		if (oddFooter != null)
		{
			sw.Write($"<oddFooter>{oddFooter}</oddFooter>");
		}
		if (evenHeader != null)
		{
			sw.Write($"<evenHeader>{evenHeader}</evenHeader>");
		}
		if (evenFooter != null)
		{
			sw.Write($"<evenFooter>{evenFooter}</evenFooter>");
		}
		if (firstHeader != null)
		{
			sw.Write($"<firstHeader>{firstHeader}</firstHeader>");
		}
		if (firstFooter != null)
		{
			sw.Write($"<firstFooter>{firstFooter}</firstFooter>");
		}
		sw.Write($"</c:{nodeName}>");
	}
}
