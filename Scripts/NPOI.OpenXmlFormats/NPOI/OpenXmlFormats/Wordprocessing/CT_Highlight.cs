using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_Highlight
{
	private ST_HighlightColor valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_HighlightColor val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	public static CT_Highlight Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Highlight cT_Highlight = new CT_Highlight();
		if (node.Attributes["w:val"] != null)
		{
			cT_Highlight.val = (ST_HighlightColor)Enum.Parse(typeof(ST_HighlightColor), node.Attributes["w:val"].Value);
		}
		return cT_Highlight;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val.ToString());
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
