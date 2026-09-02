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
public class CT_OnOff
{
	private bool valField;

	private bool valFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public bool val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
			valFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool valSpecified
	{
		get
		{
			return valFieldSpecified;
		}
		set
		{
			valFieldSpecified = value;
		}
	}

	public static CT_OnOff Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_OnOff cT_OnOff = new CT_OnOff();
		if (node.Attributes["w:val"] != null)
		{
			cT_OnOff.valField = XmlHelper.ReadBool(node.Attributes["w:val"]);
		}
		else
		{
			cT_OnOff.valField = true;
		}
		return cT_OnOff;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		if (!valField)
		{
			XmlHelper.WriteAttribute(sw, "w:val", valField, writeIfBlank: true);
		}
		sw.Write("/>");
	}

	public void UnSetVal()
	{
		val = false;
		valFieldSpecified = false;
	}

	public bool IsSetVal()
	{
		return valFieldSpecified;
	}
}
