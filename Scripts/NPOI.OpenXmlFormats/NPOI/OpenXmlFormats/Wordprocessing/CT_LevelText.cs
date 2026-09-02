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
public class CT_LevelText
{
	private string valField;

	private ST_OnOff nullField;

	private bool nullFieldSpecified;

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

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff @null
	{
		get
		{
			return nullField;
		}
		set
		{
			nullField = value;
		}
	}

	[XmlIgnore]
	public bool nullSpecified
	{
		get
		{
			return nullFieldSpecified;
		}
		set
		{
			nullFieldSpecified = value;
		}
	}

	public static CT_LevelText Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_LevelText cT_LevelText = new CT_LevelText();
		cT_LevelText.val = XmlHelper.ReadString(node.Attributes["w:val"]);
		if (node.Attributes["w:null"] != null)
		{
			cT_LevelText.@null = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:null"].Value, ignoreCase: true);
		}
		return cT_LevelText;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val);
		if (@null != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:null", @null.ToString());
		}
		sw.Write("/>");
	}
}
