using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_OMathArgPr
{
	private CT_Integer2 argSzField;

	[XmlElement(Order = 0)]
	public CT_Integer2 argSz
	{
		get
		{
			return argSzField;
		}
		set
		{
			argSzField = value;
		}
	}

	public static CT_OMathArgPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_OMathArgPr cT_OMathArgPr = new CT_OMathArgPr();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "argSz")
			{
				cT_OMathArgPr.argSz = CT_Integer2.Parse(childNode, namespaceManager);
			}
		}
		return cT_OMathArgPr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		sw.Write(">");
		if (argSz != null)
		{
			argSz.Write(sw, "argSz");
		}
		sw.Write($"</m:{nodeName}>");
	}
}
