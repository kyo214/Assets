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
public class CT_YAlign
{
	private ST_YAlign valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_YAlign val
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

	public static CT_YAlign Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_YAlign cT_YAlign = new CT_YAlign();
		if (node.Attributes["m:val"] != null)
		{
			cT_YAlign.val = (ST_YAlign)Enum.Parse(typeof(ST_YAlign), node.Attributes["m:val"].Value);
		}
		return cT_YAlign;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:val", val.ToString());
		sw.Write(">");
		sw.Write($"</m:{nodeName}>");
	}
}
