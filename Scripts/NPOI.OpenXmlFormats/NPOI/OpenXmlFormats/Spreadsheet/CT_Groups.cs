using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Groups
{
	private List<CT_LevelGroup> groupField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("group", Order = 0)]
	public List<CT_LevelGroup> group
	{
		get
		{
			return groupField;
		}
		set
		{
			groupField = value;
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

	public static CT_Groups Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Groups cT_Groups = new CT_Groups();
		if (node.Attributes["count"] != null)
		{
			cT_Groups.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_Groups.group = new List<CT_LevelGroup>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "group")
			{
				cT_Groups.group.Add(CT_LevelGroup.Parse(childNode, namespaceManager));
			}
		}
		return cT_Groups;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (group != null)
		{
			foreach (CT_LevelGroup item in group)
			{
				item.Write(sw, "group");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Groups()
	{
		groupField = new List<CT_LevelGroup>();
	}
}
