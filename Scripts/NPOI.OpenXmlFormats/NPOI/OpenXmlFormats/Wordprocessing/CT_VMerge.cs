using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_VMerge
{
	private ST_Merge valField;

	private bool valFieldSpecified;

	public ST_Merge val
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

	public CT_VMerge()
	{
		valField = ST_Merge.@continue;
	}

	public static CT_VMerge Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_VMerge cT_VMerge = new CT_VMerge();
		if (node.Attributes["w:val"] != null)
		{
			cT_VMerge.valField = (ST_Merge)Enum.Parse(typeof(ST_Merge), node.Attributes["w:val"].Value);
			cT_VMerge.valFieldSpecified = true;
		}
		return cT_VMerge;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		if (valField != ST_Merge.@continue || valFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "w:val", valField.ToString());
		}
		sw.Write("/>");
	}
}
