using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Break
{
	private uint idField;

	private uint minField;

	private uint maxField;

	private bool manField;

	private bool ptField;

	[DefaultValue(typeof(uint), "0")]
	public uint id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	[DefaultValue(typeof(uint), "0")]
	public uint min
	{
		get
		{
			return minField;
		}
		set
		{
			minField = value;
		}
	}

	[DefaultValue(typeof(uint), "0")]
	public uint max
	{
		get
		{
			return maxField;
		}
		set
		{
			maxField = value;
		}
	}

	[DefaultValue(false)]
	public bool man
	{
		get
		{
			return manField;
		}
		set
		{
			manField = value;
		}
	}

	[DefaultValue(false)]
	public bool pt
	{
		get
		{
			return ptField;
		}
		set
		{
			ptField = value;
		}
	}

	public CT_Break()
	{
		idField = 0u;
		minField = 0u;
		maxField = 0u;
		manField = false;
		ptField = false;
	}

	public static CT_Break Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Break
		{
			id = XmlHelper.ReadUInt(node.Attributes["id"]),
			min = XmlHelper.ReadUInt(node.Attributes["min"]),
			max = XmlHelper.ReadUInt(node.Attributes["max"]),
			man = XmlHelper.ReadBool(node.Attributes["man"]),
			pt = XmlHelper.ReadBool(node.Attributes["pt"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "id", id);
		XmlHelper.WriteAttribute(sw, "min", min);
		XmlHelper.WriteAttribute(sw, "max", max);
		XmlHelper.WriteAttribute(sw, "man", man);
		XmlHelper.WriteAttribute(sw, "pt", pt);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
