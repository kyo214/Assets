using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ServerFormat
{
	private string cultureField;

	private string formatField;

	[XmlAttribute]
	public string culture
	{
		get
		{
			return cultureField;
		}
		set
		{
			cultureField = value;
		}
	}

	[XmlAttribute]
	public string format
	{
		get
		{
			return formatField;
		}
		set
		{
			formatField = value;
		}
	}

	public static CT_ServerFormat Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_ServerFormat
		{
			culture = XmlHelper.ReadString(node.Attributes["culture"]),
			format = XmlHelper.ReadString(node.Attributes["format"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "culture", culture);
		XmlHelper.WriteAttribute(sw, "format", format);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
