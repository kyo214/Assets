using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_RegularTextRun
{
	private CT_TextCharacterProperties rPrField;

	private string tField;

	[XmlElement(Order = 0)]
	public CT_TextCharacterProperties rPr
	{
		get
		{
			return rPrField;
		}
		set
		{
			rPrField = value;
		}
	}

	[XmlElement(Order = 1)]
	public string t
	{
		get
		{
			return tField;
		}
		set
		{
			tField = value;
		}
	}

	public static CT_RegularTextRun Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RegularTextRun cT_RegularTextRun = new CT_RegularTextRun();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "rPr")
			{
				cT_RegularTextRun.rPr = CT_TextCharacterProperties.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "t")
			{
				cT_RegularTextRun.t = childNode.InnerText;
			}
		}
		return cT_RegularTextRun;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		sw.Write(">");
		if (rPr != null)
		{
			rPr.Write(sw, "rPr");
		}
		if (t != null)
		{
			sw.Write("<a:t>");
			sw.Write(XmlHelper.EncodeXml(t));
			sw.Write("</a:t>");
		}
		sw.Write($"</a:{nodeName}>");
	}

	public CT_TextCharacterProperties AddNewRPr()
	{
		rPrField = new CT_TextCharacterProperties();
		return rPrField;
	}

	public bool IsSetRPr()
	{
		return rPrField != null;
	}
}
