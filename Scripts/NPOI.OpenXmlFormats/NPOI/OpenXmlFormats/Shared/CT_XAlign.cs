using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_XAlign
{
	private ST_XAlign valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_XAlign val
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

	public static CT_XAlign Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_XAlign cT_XAlign = new CT_XAlign();
		if (node.Attributes["m:val"] != null)
		{
			cT_XAlign.val = (ST_XAlign)Enum.Parse(typeof(ST_XAlign), node.Attributes["m:val"].Value);
		}
		return cT_XAlign;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:val", val.ToString());
		sw.Write(">");
		sw.Write($"</m:{nodeName}>");
	}
}
