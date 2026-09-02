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
public class CT_BreakBin
{
	private ST_BreakBin valField;

	private bool valFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_BreakBin val
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
			return valField != ST_BreakBin.NONE;
		}
		set
		{
			valFieldSpecified = value;
		}
	}

	public static CT_BreakBin Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_BreakBin cT_BreakBin = new CT_BreakBin();
		if (node.Attributes["m:val"] != null)
		{
			cT_BreakBin.val = (ST_BreakBin)Enum.Parse(typeof(ST_BreakBin), node.Attributes["m:val"].Value);
		}
		return cT_BreakBin;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:val", val.ToString());
		sw.Write("/>");
	}
}
