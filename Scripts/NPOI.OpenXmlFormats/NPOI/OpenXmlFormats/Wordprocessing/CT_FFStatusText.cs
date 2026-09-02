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
public class CT_FFStatusText
{
	private ST_InfoTextType typeField;

	private bool typeFieldSpecified;

	private string valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_InfoTextType type
	{
		get
		{
			return typeField;
		}
		set
		{
			typeField = value;
		}
	}

	[XmlIgnore]
	public bool typeSpecified
	{
		get
		{
			return typeFieldSpecified;
		}
		set
		{
			typeFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string val
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

	public static CT_FFStatusText Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FFStatusText cT_FFStatusText = new CT_FFStatusText();
		if (node.Attributes["w:type"] != null)
		{
			cT_FFStatusText.typeFieldSpecified = true;
			cT_FFStatusText.typeField = (ST_InfoTextType)Enum.Parse(typeof(ST_InfoTextType), node.Attributes["w:type"].Value);
		}
		cT_FFStatusText.valField = XmlHelper.ReadString(node.Attributes["w:val"]);
		return cT_FFStatusText;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", valField);
		if (typeFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "w:type", typeField.ToString());
		}
		sw.Write("/>");
	}
}
