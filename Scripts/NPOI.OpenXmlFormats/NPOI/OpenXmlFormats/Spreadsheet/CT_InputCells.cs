using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_InputCells
{
	private string rField;

	private bool deletedField;

	private bool undoneField;

	private string valField;

	private uint numFmtIdField;

	private bool numFmtIdFieldSpecified;

	public string r
	{
		get
		{
			return rField;
		}
		set
		{
			rField = value;
		}
	}

	[DefaultValue(false)]
	public bool deleted
	{
		get
		{
			return deletedField;
		}
		set
		{
			deletedField = value;
		}
	}

	[DefaultValue(false)]
	public bool undone
	{
		get
		{
			return undoneField;
		}
		set
		{
			undoneField = value;
		}
	}

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

	public uint numFmtId
	{
		get
		{
			return numFmtIdField;
		}
		set
		{
			numFmtIdField = value;
		}
	}

	public CT_InputCells()
	{
		deletedField = false;
		undoneField = false;
	}

	public static CT_InputCells Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_InputCells
		{
			r = XmlHelper.ReadString(node.Attributes["r"]),
			deleted = XmlHelper.ReadBool(node.Attributes["deleted"]),
			undone = XmlHelper.ReadBool(node.Attributes["undone"]),
			val = XmlHelper.ReadString(node.Attributes["val"]),
			numFmtId = XmlHelper.ReadUInt(node.Attributes["numFmtId"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "r", r);
		XmlHelper.WriteAttribute(sw, "deleted", deleted);
		XmlHelper.WriteAttribute(sw, "undone", undone);
		XmlHelper.WriteAttribute(sw, "val", val);
		XmlHelper.WriteAttribute(sw, "numFmtId", numFmtId, writeIfBlank: true);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
