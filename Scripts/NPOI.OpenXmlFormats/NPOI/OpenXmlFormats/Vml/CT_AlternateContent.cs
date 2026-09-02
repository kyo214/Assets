using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Vml;

public class CT_AlternateContent
{
	public string InnerXml { get; set; }

	public static CT_AlternateContent Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_AlternateContent cT_AlternateContent = new CT_AlternateContent();
		if (string.IsNullOrEmpty(node.InnerXml))
		{
			return cT_AlternateContent;
		}
		cT_AlternateContent.InnerXml = node.InnerXml;
		return cT_AlternateContent;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<mc:{nodeName} xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"");
		if (InnerXml == null)
		{
			sw.Write(string.Format("/>", nodeName));
			return;
		}
		sw.Write(">");
		sw.Write(InnerXml);
		sw.Write($"</mc:{nodeName}>");
	}
}
