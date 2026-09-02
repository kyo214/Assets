using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Members
{
	private List<CT_Member> memberField;

	private uint countField;

	private bool countFieldSpecified;

	private uint levelField;

	private bool levelFieldSpecified;

	[XmlElement("member", Order = 0)]
	public List<CT_Member> member
	{
		get
		{
			return memberField;
		}
		set
		{
			memberField = value;
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

	[XmlAttribute]
	public uint level
	{
		get
		{
			return levelField;
		}
		set
		{
			levelField = value;
		}
	}

	[XmlIgnore]
	public bool levelSpecified
	{
		get
		{
			return levelFieldSpecified;
		}
		set
		{
			levelFieldSpecified = value;
		}
	}

	public static CT_Members Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Members cT_Members = new CT_Members();
		if (node.Attributes["count"] != null)
		{
			cT_Members.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		if (node.Attributes["level"] != null)
		{
			cT_Members.level = XmlHelper.ReadUInt(node.Attributes["level"]);
		}
		cT_Members.member = new List<CT_Member>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "member")
			{
				cT_Members.member.Add(CT_Member.Parse(childNode, namespaceManager));
			}
		}
		return cT_Members;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		XmlHelper.WriteAttribute(sw, "level", level);
		sw.Write(">");
		if (member != null)
		{
			foreach (CT_Member item in member)
			{
				item.Write(sw, "member");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Members()
	{
		memberField = new List<CT_Member>();
	}
}
