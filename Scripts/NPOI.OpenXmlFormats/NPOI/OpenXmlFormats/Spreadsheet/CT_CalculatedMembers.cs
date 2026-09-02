using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CalculatedMembers
{
	private List<CT_CalculatedMember> calculatedMemberField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("calculatedMember", Order = 0)]
	public List<CT_CalculatedMember> calculatedMember
	{
		get
		{
			return calculatedMemberField;
		}
		set
		{
			calculatedMemberField = value;
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

	public static CT_CalculatedMembers Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CalculatedMembers cT_CalculatedMembers = new CT_CalculatedMembers();
		if (node.Attributes["count"] != null)
		{
			cT_CalculatedMembers.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_CalculatedMembers.calculatedMember = new List<CT_CalculatedMember>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "calculatedMember")
			{
				cT_CalculatedMembers.calculatedMember.Add(CT_CalculatedMember.Parse(childNode, namespaceManager));
			}
		}
		return cT_CalculatedMembers;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (calculatedMember != null)
		{
			foreach (CT_CalculatedMember item in calculatedMember)
			{
				item.Write(sw, "calculatedMember");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CalculatedMembers()
	{
		calculatedMemberField = new List<CT_CalculatedMember>();
	}
}
