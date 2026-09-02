using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Items
{
	private List<CT_Item> itemField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("item", Order = 0)]
	public List<CT_Item> item
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

	public static CT_Items Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Items cT_Items = new CT_Items();
		if (node.Attributes["count"] != null)
		{
			cT_Items.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_Items.item = new List<CT_Item>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "item")
			{
				cT_Items.item.Add(CT_Item.Parse(childNode, namespaceManager));
			}
		}
		return cT_Items;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (item != null)
		{
			foreach (CT_Item item in item)
			{
				item.Write(sw, "item");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Items()
	{
		itemField = new List<CT_Item>();
	}

	public CT_Item AddNewItem()
	{
		if (itemField == null)
		{
			itemField = new List<CT_Item>();
		}
		CT_Item result = new CT_Item();
		itemField.Add(result);
		return result;
	}

	public uint SizeOfItemArray()
	{
		if (itemField == null)
		{
			itemField = new List<CT_Item>();
		}
		return (uint)itemField.Count;
	}
}
