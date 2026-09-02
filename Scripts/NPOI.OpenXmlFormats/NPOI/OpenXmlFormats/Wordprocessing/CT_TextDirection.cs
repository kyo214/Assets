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
public class CT_TextDirection
{
	private ST_TextDirection valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_TextDirection val
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

	public static CT_TextDirection Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TextDirection cT_TextDirection = new CT_TextDirection();
		if (node.Attributes["w:val"] != null)
		{
			cT_TextDirection.val = (ST_TextDirection)Enum.Parse(typeof(ST_TextDirection), node.Attributes["w:val"].Value);
		}
		return cT_TextDirection;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val.ToString());
		sw.Write("/>");
	}
}
