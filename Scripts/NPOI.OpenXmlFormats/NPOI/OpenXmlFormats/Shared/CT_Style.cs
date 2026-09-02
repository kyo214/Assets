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
public class CT_Style
{
	private ST_Style valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_Style val
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

	public static CT_Style Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Style cT_Style = new CT_Style();
		if (node.Attributes["m:val"] != null)
		{
			cT_Style.val = (ST_Style)Enum.Parse(typeof(ST_Style), node.Attributes["m:val"].Value);
		}
		return cT_Style;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:val", val.ToString());
		sw.Write("/>");
	}
}
