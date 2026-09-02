using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_LevelGroup
{
	private CT_GroupMembers groupMembersField;

	private string nameField;

	private string uniqueNameField;

	private string captionField;

	private string uniqueParentField;

	private int idField;

	private bool idFieldSpecified;

	[XmlElement(Order = 0)]
	public CT_GroupMembers groupMembers
	{
		get
		{
			return groupMembersField;
		}
		set
		{
			groupMembersField = value;
		}
	}

	[XmlAttribute]
	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	[XmlAttribute]
	public string uniqueName
	{
		get
		{
			return uniqueNameField;
		}
		set
		{
			uniqueNameField = value;
		}
	}

	[XmlAttribute]
	public string caption
	{
		get
		{
			return captionField;
		}
		set
		{
			captionField = value;
		}
	}

	[XmlAttribute]
	public string uniqueParent
	{
		get
		{
			return uniqueParentField;
		}
		set
		{
			uniqueParentField = value;
		}
	}

	[XmlAttribute]
	public int id
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

	[XmlIgnore]
	public bool idSpecified
	{
		get
		{
			return idFieldSpecified;
		}
		set
		{
			idFieldSpecified = value;
		}
	}

	public static CT_LevelGroup Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_LevelGroup cT_LevelGroup = new CT_LevelGroup();
		cT_LevelGroup.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_LevelGroup.uniqueName = XmlHelper.ReadString(node.Attributes["uniqueName"]);
		cT_LevelGroup.caption = XmlHelper.ReadString(node.Attributes["caption"]);
		cT_LevelGroup.uniqueParent = XmlHelper.ReadString(node.Attributes["uniqueParent"]);
		if (node.Attributes["id"] != null)
		{
			cT_LevelGroup.id = XmlHelper.ReadInt(node.Attributes["id"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "groupMembers")
			{
				cT_LevelGroup.groupMembers = CT_GroupMembers.Parse(childNode, namespaceManager);
			}
		}
		return cT_LevelGroup;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "uniqueName", uniqueName);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		XmlHelper.WriteAttribute(sw, "uniqueParent", uniqueParent);
		XmlHelper.WriteAttribute(sw, "id", id);
		sw.Write(">");
		if (groupMembers != null)
		{
			groupMembers.Write(sw, "groupMembers");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_LevelGroup()
	{
		groupMembersField = new CT_GroupMembers();
	}
}
