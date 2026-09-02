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
public class CT_ShortHexNumber
{
	private byte[] valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "hexBinary")]
	public byte[] val
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

	public static CT_ShortHexNumber Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_ShortHexNumber
		{
			val = XmlHelper.ReadBytes(node.Attributes["w:val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val);
		sw.Write("/>");
	}
}
