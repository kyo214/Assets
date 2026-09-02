using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_GroupMembers
{
	private List<CT_GroupMember> groupMemberField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("groupMember", Order = 0)]
	public List<CT_GroupMember> groupMember
	{
		get
		{
			return groupMemberField;
		}
		set
		{
			groupMemberField = value;
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

	public static CT_GroupMembers Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GroupMembers cT_GroupMembers = new CT_GroupMembers();
		if (node.Attributes["count"] != null)
		{
			cT_GroupMembers.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_GroupMembers.groupMember = new List<CT_GroupMember>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "groupMember")
			{
				cT_GroupMembers.groupMember.Add(CT_GroupMember.Parse(childNode, namespaceManager));
			}
		}
		return cT_GroupMembers;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (groupMember != null)
		{
			foreach (CT_GroupMember item in groupMember)
			{
				item.Write(sw, "groupMember");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_GroupMembers()
	{
		groupMemberField = new List<CT_GroupMember>();
	}
}
