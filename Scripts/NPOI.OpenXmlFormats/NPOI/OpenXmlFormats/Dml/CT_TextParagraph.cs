using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_TextParagraph
{
	private CT_TextParagraphProperties pPrField;

	private List<CT_RegularTextRun> rField = new List<CT_RegularTextRun>();

	private List<CT_TextLineBreak> brField = new List<CT_TextLineBreak>();

	private List<CT_TextField> fldField = new List<CT_TextField>();

	private List<object> itemsField = new List<object>();

	private CT_TextCharacterProperties endParaRPrField;

	[XmlIgnore]
	public List<object> items
	{
		get
		{
			return itemsField;
		}
		set
		{
			itemsField = value;
		}
	}

	public CT_TextParagraphProperties pPr
	{
		get
		{
			return pPrField;
		}
		set
		{
			pPrField = value;
		}
	}

	[XmlElement("r")]
	public List<CT_RegularTextRun> r
	{
		get
		{
			return rField;
		}
		set
		{
			rField = value;
		}
	}

	[XmlElement("br")]
	public List<CT_TextLineBreak> br
	{
		get
		{
			return brField;
		}
		set
		{
			brField = value;
		}
	}

	[XmlElement("fld")]
	public List<CT_TextField> fld
	{
		get
		{
			return fldField;
		}
		set
		{
			fldField = value;
		}
	}

	public CT_TextCharacterProperties endParaRPr
	{
		get
		{
			return endParaRPrField;
		}
		set
		{
			endParaRPrField = value;
		}
	}

	public static CT_TextParagraph Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TextParagraph cT_TextParagraph = new CT_TextParagraph();
		cT_TextParagraph.r = new List<CT_RegularTextRun>();
		cT_TextParagraph.br = new List<CT_TextLineBreak>();
		cT_TextParagraph.fld = new List<CT_TextField>();
		cT_TextParagraph.items = new List<object>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pPr")
			{
				cT_TextParagraph.pPr = CT_TextParagraphProperties.Parse(childNode, namespaceManager);
				cT_TextParagraph.items.Add(cT_TextParagraph.pPr);
			}
			else if (childNode.LocalName == "endParaRPr")
			{
				cT_TextParagraph.endParaRPr = CT_TextCharacterProperties.Parse(childNode, namespaceManager);
				cT_TextParagraph.items.Add(cT_TextParagraph.endParaRPr);
			}
			else if (childNode.LocalName == "r")
			{
				CT_RegularTextRun item = CT_RegularTextRun.Parse(childNode, namespaceManager);
				cT_TextParagraph.r.Add(item);
				cT_TextParagraph.items.Add(item);
			}
			else if (childNode.LocalName == "br")
			{
				CT_TextLineBreak item2 = CT_TextLineBreak.Parse(childNode, namespaceManager);
				cT_TextParagraph.br.Add(item2);
				cT_TextParagraph.items.Add(item2);
			}
			else if (childNode.LocalName == "fld")
			{
				CT_TextField item3 = CT_TextField.Parse(childNode, namespaceManager);
				cT_TextParagraph.fld.Add(item3);
				cT_TextParagraph.items.Add(item3);
			}
		}
		return cT_TextParagraph;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		sw.Write(">");
		if (pPr != null)
		{
			pPr.Write(sw, "pPr");
		}
		foreach (object item in items)
		{
			if (item is CT_RegularTextRun)
			{
				(item as CT_RegularTextRun).Write(sw, "r");
			}
			else if (item is CT_TextLineBreak)
			{
				(item as CT_TextLineBreak).Write(sw, "br");
			}
			else if (item is CT_TextField)
			{
				(item as CT_TextField).Write(sw, "fld");
			}
		}
		if (endParaRPr != null)
		{
			endParaRPr.Write(sw, "endParaRPr");
		}
		sw.Write($"</a:{nodeName}>");
	}

	public CT_RegularTextRun AddNewR()
	{
		if (rField == null)
		{
			rField = new List<CT_RegularTextRun>();
		}
		CT_RegularTextRun cT_RegularTextRun = new CT_RegularTextRun();
		rField.Add(cT_RegularTextRun);
		itemsField.Add(cT_RegularTextRun);
		return cT_RegularTextRun;
	}

	public CT_TextParagraphProperties AddNewPPr()
	{
		pPrField = new CT_TextParagraphProperties();
		return pPrField;
	}

	public CT_TextCharacterProperties AddNewEndParaRPr()
	{
		endParaRPrField = new CT_TextCharacterProperties();
		return endParaRPrField;
	}

	public int SizeOfRArray()
	{
		return rField.Count;
	}

	public bool IsSetPPr()
	{
		return pPrField != null;
	}

	public CT_TextLineBreak AddNewBr()
	{
		CT_TextLineBreak cT_TextLineBreak = new CT_TextLineBreak();
		brField.Add(cT_TextLineBreak);
		itemsField.Add(cT_TextLineBreak);
		return cT_TextLineBreak;
	}

	public CT_RegularTextRun GetRArray(int v)
	{
		return rField[v];
	}

	public int SizeOfFldArray()
	{
		return fldField.Count;
	}

	public CT_TextField GetFldArray(int v)
	{
		return fldField[v];
	}
}
