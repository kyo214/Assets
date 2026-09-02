using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Query
{
	private CT_Tuples tplsField;

	private string mdxField;

	[XmlElement(Order = 0)]
	public CT_Tuples tpls
	{
		get
		{
			return tplsField;
		}
		set
		{
			tplsField = value;
		}
	}

	[XmlAttribute]
	public string mdx
	{
		get
		{
			return mdxField;
		}
		set
		{
			mdxField = value;
		}
	}

	public static CT_Query Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Query cT_Query = new CT_Query();
		cT_Query.mdx = XmlHelper.ReadString(node.Attributes["mdx"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "tpls")
			{
				cT_Query.tpls = CT_Tuples.Parse(childNode, namespaceManager);
			}
		}
		return cT_Query;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "mdx", mdx);
		sw.Write(">");
		if (tpls != null)
		{
			tpls.Write(sw, "tpls");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Query()
	{
		tplsField = new CT_Tuples();
	}
}
