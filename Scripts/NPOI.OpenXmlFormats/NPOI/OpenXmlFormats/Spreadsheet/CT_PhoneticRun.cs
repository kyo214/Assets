using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", ElementName = "rPh", IsNullable = false)]
public class CT_PhoneticRun
{
	private string tField;

	private uint sbField;

	private uint ebField;

	[XmlAttribute]
	public string t
	{
		get
		{
			return tField;
		}
		set
		{
			tField = value;
		}
	}

	[XmlAttribute]
	public uint sb
	{
		get
		{
			return sbField;
		}
		set
		{
			sbField = value;
		}
	}

	[XmlAttribute]
	public uint eb
	{
		get
		{
			return ebField;
		}
		set
		{
			ebField = value;
		}
	}

	public static CT_PhoneticRun Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PhoneticRun cT_PhoneticRun = new CT_PhoneticRun();
		cT_PhoneticRun.sb = XmlHelper.ReadUInt(node.Attributes["sb"]);
		cT_PhoneticRun.eb = XmlHelper.ReadUInt(node.Attributes["eb"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "t")
			{
				cT_PhoneticRun.t = childNode.InnerText;
			}
		}
		return cT_PhoneticRun;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "sb", sb.ToString(), writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "eb", eb.ToString(), writeIfBlank: true);
		sw.Write(">");
		sw.Write("<t>");
		if (t != null)
		{
			sw.Write(XmlHelper.EncodeXml(t));
		}
		sw.Write("</t>");
		sw.Write($"</{nodeName}>");
	}
}
