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
public class CT_Column
{
	private ulong wField;

	private bool wFieldSpecified;

	private ulong spaceField;

	private bool spaceFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong w
	{
		get
		{
			return wField;
		}
		set
		{
			wField = value;
		}
	}

	[XmlIgnore]
	public bool wSpecified
	{
		get
		{
			return wFieldSpecified;
		}
		set
		{
			wFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong space
	{
		get
		{
			return spaceField;
		}
		set
		{
			spaceField = value;
		}
	}

	[XmlIgnore]
	public bool spaceSpecified
	{
		get
		{
			return spaceFieldSpecified;
		}
		set
		{
			spaceFieldSpecified = value;
		}
	}

	public static CT_Column Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Column
		{
			w = XmlHelper.ReadULong(node.Attributes["w:w"]),
			space = XmlHelper.ReadULong(node.Attributes["w:space"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:w", w);
		XmlHelper.WriteAttribute(sw, "w:space", space);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
