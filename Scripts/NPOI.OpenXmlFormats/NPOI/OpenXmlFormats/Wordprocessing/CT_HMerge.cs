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
public class CT_HMerge
{
	private ST_Merge valField;

	private bool valFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_Merge val
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

	public static CT_HMerge Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_HMerge cT_HMerge = new CT_HMerge();
		if (node.Attributes["w:val"] != null)
		{
			cT_HMerge.val = (ST_Merge)Enum.Parse(typeof(ST_Merge), node.Attributes["w:val"].Value);
		}
		return cT_HMerge;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:val", val.ToString());
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
