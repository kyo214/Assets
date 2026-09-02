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
public class CT_TwipsMeasure
{
	private uint valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public uint val
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

	public static CT_TwipsMeasure Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TwipsMeasure
		{
			val = XmlHelper.ReadUInt(node.Attributes["m:val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:val", val, writeIfBlank: true);
		sw.Write("/>");
	}
}
