using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PCDSDTCEntries
{
	private List<object> itemsField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("e", typeof(CT_Error), Order = 0)]
	[XmlElement("m", typeof(CT_Missing), Order = 0)]
	[XmlElement("n", typeof(CT_Number), Order = 0)]
	[XmlElement("s", typeof(CT_String), Order = 0)]
	public List<object> Items
	{
		get
		{
			return itemsField;
		}
		set
		{
			itemsField = value;
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

	public static CT_PCDSDTCEntries Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PCDSDTCEntries cT_PCDSDTCEntries = new CT_PCDSDTCEntries();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "m")
			{
				cT_PCDSDTCEntries.Items.Add(CT_Missing.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "n")
			{
				cT_PCDSDTCEntries.Items.Add(CT_Number.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "e")
			{
				cT_PCDSDTCEntries.Items.Add(CT_Error.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "s")
			{
				cT_PCDSDTCEntries.Items.Add(CT_String.Parse(childNode, namespaceManager));
			}
		}
		return cT_PCDSDTCEntries;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		sw.Write(">");
		foreach (object item in Items)
		{
			if (item is CT_Missing)
			{
				((CT_Missing)item).Write(sw, "m");
			}
			else if (item is CT_Number)
			{
				((CT_Number)item).Write(sw, "n");
			}
			else if (item is CT_Error)
			{
				((CT_Error)item).Write(sw, "e");
			}
			else if (item is CT_String)
			{
				((CT_String)item).Write(sw, "s");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PCDSDTCEntries()
	{
		itemsField = new List<object>();
	}
}
