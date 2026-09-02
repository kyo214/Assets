using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_GroupLevel
{
	private CT_Groups groupsField;

	private CT_ExtensionList extLstField;

	private string uniqueNameField;

	private string captionField;

	private bool userField;

	private bool customRollUpField;

	[XmlElement(Order = 0)]
	public CT_Groups groups
	{
		get
		{
			return groupsField;
		}
		set
		{
			groupsField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_ExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
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
	[DefaultValue(false)]
	public bool user
	{
		get
		{
			return userField;
		}
		set
		{
			userField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool customRollUp
	{
		get
		{
			return customRollUpField;
		}
		set
		{
			customRollUpField = value;
		}
	}

	public static CT_GroupLevel Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GroupLevel cT_GroupLevel = new CT_GroupLevel();
		cT_GroupLevel.uniqueName = XmlHelper.ReadString(node.Attributes["uniqueName"]);
		cT_GroupLevel.caption = XmlHelper.ReadString(node.Attributes["caption"]);
		if (node.Attributes["user"] != null)
		{
			cT_GroupLevel.user = XmlHelper.ReadBool(node.Attributes["user"]);
		}
		if (node.Attributes["customRollUp"] != null)
		{
			cT_GroupLevel.customRollUp = XmlHelper.ReadBool(node.Attributes["customRollUp"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "groups")
			{
				cT_GroupLevel.groups = CT_Groups.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_GroupLevel.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_GroupLevel;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "uniqueName", uniqueName);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		XmlHelper.WriteAttribute(sw, "user", user);
		XmlHelper.WriteAttribute(sw, "customRollUp", customRollUp);
		sw.Write(">");
		if (groups != null)
		{
			groups.Write(sw, "groups");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_GroupLevel()
	{
		extLstField = new CT_ExtensionList();
		groupsField = new CT_Groups();
		userField = false;
		customRollUpField = false;
	}
}
