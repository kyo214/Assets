using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CalculatedMember
{
	private CT_ExtensionList extLstField;

	private string nameField;

	private string mdxField;

	private string memberNameField;

	private string hierarchyField;

	private string parentField;

	private int solveOrderField;

	private bool setField;

	[XmlElement(Order = 0)]
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
	public string mdx
	{
		get
		{
			return mdxField;
		}
		set
		{
			mdxField = value;
		}
	}

	[XmlAttribute]
	public string memberName
	{
		get
		{
			return memberNameField;
		}
		set
		{
			memberNameField = value;
		}
	}

	[XmlAttribute]
	public string hierarchy
	{
		get
		{
			return hierarchyField;
		}
		set
		{
			hierarchyField = value;
		}
	}

	[XmlAttribute]
	public string parent
	{
		get
		{
			return parentField;
		}
		set
		{
			parentField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int solveOrder
	{
		get
		{
			return solveOrderField;
		}
		set
		{
			solveOrderField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool set
	{
		get
		{
			return setField;
		}
		set
		{
			setField = value;
		}
	}

	public static CT_CalculatedMember Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CalculatedMember cT_CalculatedMember = new CT_CalculatedMember();
		cT_CalculatedMember.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_CalculatedMember.mdx = XmlHelper.ReadString(node.Attributes["mdx"]);
		cT_CalculatedMember.memberName = XmlHelper.ReadString(node.Attributes["memberName"]);
		cT_CalculatedMember.hierarchy = XmlHelper.ReadString(node.Attributes["hierarchy"]);
		cT_CalculatedMember.parent = XmlHelper.ReadString(node.Attributes["parent"]);
		if (node.Attributes["solveOrder"] != null)
		{
			cT_CalculatedMember.solveOrder = XmlHelper.ReadInt(node.Attributes["solveOrder"]);
		}
		if (node.Attributes["set"] != null)
		{
			cT_CalculatedMember.set = XmlHelper.ReadBool(node.Attributes["set"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_CalculatedMember.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_CalculatedMember;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "mdx", mdx);
		XmlHelper.WriteAttribute(sw, "memberName", memberName);
		XmlHelper.WriteAttribute(sw, "hierarchy", hierarchy);
		XmlHelper.WriteAttribute(sw, "parent", parent);
		XmlHelper.WriteAttribute(sw, "solveOrder", solveOrder);
		XmlHelper.WriteAttribute(sw, "set", set);
		sw.Write(">");
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CalculatedMember()
	{
		extLstField = new CT_ExtensionList();
		solveOrderField = 0;
		setField = false;
	}
}
