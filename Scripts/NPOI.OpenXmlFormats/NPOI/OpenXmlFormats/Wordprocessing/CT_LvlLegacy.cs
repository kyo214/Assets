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
public class CT_LvlLegacy
{
	private ST_OnOff legacyField;

	private bool legacyFieldSpecified;

	private ulong legacySpaceField;

	private bool legacySpaceFieldSpecified;

	private string legacyIndentField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff legacy
	{
		get
		{
			return legacyField;
		}
		set
		{
			legacyField = value;
		}
	}

	[XmlIgnore]
	public bool legacySpecified
	{
		get
		{
			return legacyFieldSpecified;
		}
		set
		{
			legacyFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong legacySpace
	{
		get
		{
			return legacySpaceField;
		}
		set
		{
			legacySpaceField = value;
		}
	}

	[XmlIgnore]
	public bool legacySpaceSpecified
	{
		get
		{
			return legacySpaceFieldSpecified;
		}
		set
		{
			legacySpaceFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "integer")]
	public string legacyIndent
	{
		get
		{
			return legacyIndentField;
		}
		set
		{
			legacyIndentField = value;
		}
	}

	public static CT_LvlLegacy Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_LvlLegacy cT_LvlLegacy = new CT_LvlLegacy();
		if (node.Attributes["w:legacy"] != null)
		{
			cT_LvlLegacy.legacy = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:legacy"].Value, ignoreCase: true);
		}
		cT_LvlLegacy.legacySpace = XmlHelper.ReadULong(node.Attributes["w:legacySpace"]);
		cT_LvlLegacy.legacyIndent = XmlHelper.ReadString(node.Attributes["w:legacyIndent"]);
		return cT_LvlLegacy;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:legacy", legacy.ToString());
		XmlHelper.WriteAttribute(sw, "w:legacySpace", legacySpace);
		XmlHelper.WriteAttribute(sw, "w:legacyIndent", legacyIndent);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
