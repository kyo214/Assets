using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CalculatedItems
{
	private List<CT_CalculatedItem> calculatedItemField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("calculatedItem", Order = 0)]
	public List<CT_CalculatedItem> calculatedItem
	{
		get
		{
			return calculatedItemField;
		}
		set
		{
			calculatedItemField = value;
		}
	}

	[XmlAttribute]
	public uint count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
		}
	}

	[XmlIgnore]
	public bool countSpecified
	{
		get
		{
			return countFieldSpecified;
		}
		set
		{
			countFieldSpecified = value;
		}
	}

	public static CT_CalculatedItems Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CalculatedItems cT_CalculatedItems = new CT_CalculatedItems();
		if (node.Attributes["count"] != null)
		{
			cT_CalculatedItems.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_CalculatedItems.calculatedItem = new List<CT_CalculatedItem>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "calculatedItem")
			{
				cT_CalculatedItems.calculatedItem.Add(CT_CalculatedItem.Parse(childNode, namespaceManager));
			}
		}
		return cT_CalculatedItems;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (calculatedItem != null)
		{
			foreach (CT_CalculatedItem item in calculatedItem)
			{
				item.Write(sw, "calculatedItem");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CalculatedItems()
	{
		calculatedItemField = new List<CT_CalculatedItem>();
	}
}
