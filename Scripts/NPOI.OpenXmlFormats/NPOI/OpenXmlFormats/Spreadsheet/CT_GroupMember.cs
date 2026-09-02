using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_GroupMember
{
	private string uniqueNameField;

	private bool groupField;

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
	[DefaultValue(false)]
	public bool group
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

	public static CT_GroupMember Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GroupMember cT_GroupMember = new CT_GroupMember();
		cT_GroupMember.uniqueName = XmlHelper.ReadString(node.Attributes["uniqueName"]);
		if (node.Attributes["group"] != null)
		{
			cT_GroupMember.group = XmlHelper.ReadBool(node.Attributes["group"]);
		}
		return cT_GroupMember;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "uniqueName", uniqueName);
		XmlHelper.WriteAttribute(sw, "group", group);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_GroupMember()
	{
		groupField = false;
	}
}
