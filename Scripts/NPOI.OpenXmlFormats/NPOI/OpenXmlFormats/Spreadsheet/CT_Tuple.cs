using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Tuple
{
	private uint fldField;

	private bool fldFieldSpecified;

	private uint hierField;

	private bool hierFieldSpecified;

	private uint itemField;

	[XmlAttribute]
	public uint fld
	{
		get
		{
			return fldField;
		}
		set
		{
			fldField = value;
		}
	}

	[XmlIgnore]
	public bool fldSpecified
	{
		get
		{
			return fldFieldSpecified;
		}
		set
		{
			fldFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint hier
	{
		get
		{
			return hierField;
		}
		set
		{
			hierField = value;
		}
	}

	[XmlIgnore]
	public bool hierSpecified
	{
		get
		{
			return hierFieldSpecified;
		}
		set
		{
			hierFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}

	public static CT_Tuple Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Tuple cT_Tuple = new CT_Tuple();
		if (node.Attributes["fld"] != null)
		{
			cT_Tuple.fld = XmlHelper.ReadUInt(node.Attributes["fld"]);
		}
		if (node.Attributes["hier"] != null)
		{
			cT_Tuple.hier = XmlHelper.ReadUInt(node.Attributes["hier"]);
		}
		if (node.Attributes["item"] != null)
		{
			cT_Tuple.item = XmlHelper.ReadUInt(node.Attributes["item"]);
		}
		return cT_Tuple;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "fld", fld);
		XmlHelper.WriteAttribute(sw, "hier", hier);
		XmlHelper.WriteAttribute(sw, "item", item);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
