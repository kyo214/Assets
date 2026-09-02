using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_GroupLevels
{
	private List<CT_GroupLevel> groupLevelField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("groupLevel", Order = 0)]
	public List<CT_GroupLevel> groupLevel
	{
		get
		{
			return groupLevelField;
		}
		set
		{
			groupLevelField = value;
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

	public static CT_GroupLevels Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GroupLevels cT_GroupLevels = new CT_GroupLevels();
		if (node.Attributes["count"] != null)
		{
			cT_GroupLevels.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_GroupLevels.groupLevel = new List<CT_GroupLevel>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "groupLevel")
			{
				cT_GroupLevels.groupLevel.Add(CT_GroupLevel.Parse(childNode, namespaceManager));
			}
		}
		return cT_GroupLevels;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (groupLevel != null)
		{
			foreach (CT_GroupLevel item in groupLevel)
			{
				item.Write(sw, "groupLevel");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_GroupLevels()
	{
		groupLevelField = new List<CT_GroupLevel>();
	}
}
