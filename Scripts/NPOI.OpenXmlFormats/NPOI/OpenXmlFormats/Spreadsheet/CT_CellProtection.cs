using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_CellProtection
{
	private bool lockedField;

	private bool hiddenField;

	[XmlAttribute]
	public bool locked
	{
		get
		{
			return lockedField;
		}
		set
		{
			lockedField = value;
		}
	}

	[XmlAttribute]
	public bool hidden
	{
		get
		{
			return hiddenField;
		}
		set
		{
			hiddenField = value;
		}
	}

	public bool IsSetHidden()
	{
		return hiddenField;
	}

	public bool IsSetLocked()
	{
		return lockedField;
	}

	public static CT_CellProtection Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_CellProtection
		{
			locked = XmlHelper.ReadBool(node.Attributes["locked"]),
			hidden = XmlHelper.ReadBool(node.Attributes["hidden"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "locked", locked);
		if (hidden)
		{
			XmlHelper.WriteAttribute(sw, "hidden", hidden);
		}
		sw.Write("/>");
	}
}
