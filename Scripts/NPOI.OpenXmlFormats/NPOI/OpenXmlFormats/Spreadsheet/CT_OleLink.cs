using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_OleLink
{
	private List<CT_OleItem> oleItemsField;

	private string idField;

	private string progIdField;

	public List<CT_OleItem> oleItems
	{
		get
		{
			return oleItemsField;
		}
		set
		{
			oleItemsField = value;
		}
	}

	public string id
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

	[XmlAttribute]
	public string progId
	{
		get
		{
			return progIdField;
		}
		set
		{
			progIdField = value;
		}
	}

	internal static CT_OleLink Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_OleLink cT_OleLink = new CT_OleLink();
		cT_OleLink.idField = XmlHelper.ReadString(node.Attributes["r:id"]);
		cT_OleLink.progIdField = XmlHelper.ReadString(node.Attributes["progId"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (!(childNode.LocalName == "oleItems"))
			{
				continue;
			}
			cT_OleLink.oleItemsField = new List<CT_OleItem>();
			foreach (XmlNode childNode2 in childNode.ChildNodes)
			{
				cT_OleLink.oleItems.Add(CT_OleItem.Parse(childNode2));
			}
		}
		return cT_OleLink;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "r:id", idField);
		XmlHelper.WriteAttribute(sw, "progId", progIdField);
		sw.Write(">");
		if (oleItemsField.Count > 0)
		{
			sw.Write("<oleItems>");
			foreach (CT_OleItem item in oleItemsField)
			{
				item.Write(sw, "oleItem");
			}
			sw.Write("</oleItems>");
		}
		sw.Write($"</{nodeName}>");
	}
}
