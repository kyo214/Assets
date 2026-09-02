using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_SSupPr
{
	private CT_CtrlPr ctrlPrField;

	[XmlElement(Order = 0)]
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

	public static CT_SSupPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_SSupPr cT_SSupPr = new CT_SSupPr();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "ctrlPr")
			{
				cT_SSupPr.ctrlPr = CT_CtrlPr.Parse(childNode, namespaceManager);
			}
		}
		return cT_SSupPr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		sw.Write(">");
		if (ctrlPr != null)
		{
			ctrlPr.Write(sw, "ctrlPr");
		}
		sw.Write($"</m:{nodeName}>");
	}
}
