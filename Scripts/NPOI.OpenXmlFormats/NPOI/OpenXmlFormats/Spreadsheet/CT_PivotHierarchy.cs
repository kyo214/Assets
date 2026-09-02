using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotHierarchy
{
	private CT_MemberProperties mpsField;

	private List<CT_Members> membersField;

	private CT_ExtensionList extLstField;

	private bool outlineField;

	private bool multipleItemSelectionAllowedField;

	private bool subtotalTopField;

	private bool showInFieldListField;

	private bool dragToRowField;

	private bool dragToColField;

	private bool dragToPageField;

	private bool dragToDataField;

	private bool dragOffField;

	private bool includeNewItemsInFilterField;

	private string captionField;

	[XmlElement(Order = 0)]
	public CT_MemberProperties mps
	{
		get
		{
			return mpsField;
		}
		set
		{
			mpsField = value;
		}
	}

	[XmlElement("members", Order = 1)]
	public List<CT_Members> members
	{
		get
		{
			return membersField;
		}
		set
		{
			membersField = value;
		}
	}

	[XmlElement(Order = 2)]
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
	[DefaultValue(false)]
	public bool outline
	{
		get
		{
			return outlineField;
		}
		set
		{
			outlineField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool multipleItemSelectionAllowed
	{
		get
		{
			return multipleItemSelectionAllowedField;
		}
		set
		{
			multipleItemSelectionAllowedField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool subtotalTop
	{
		get
		{
			return subtotalTopField;
		}
		set
		{
			subtotalTopField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showInFieldList
	{
		get
		{
			return showInFieldListField;
		}
		set
		{
			showInFieldListField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToRow
	{
		get
		{
			return dragToRowField;
		}
		set
		{
			dragToRowField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToCol
	{
		get
		{
			return dragToColField;
		}
		set
		{
			dragToColField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToPage
	{
		get
		{
			return dragToPageField;
		}
		set
		{
			dragToPageField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool dragToData
	{
		get
		{
			return dragToDataField;
		}
		set
		{
			dragToDataField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragOff
	{
		get
		{
			return dragOffField;
		}
		set
		{
			dragOffField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool includeNewItemsInFilter
	{
		get
		{
			return includeNewItemsInFilterField;
		}
		set
		{
			includeNewItemsInFilterField = value;
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

	public static CT_PivotHierarchy Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotHierarchy cT_PivotHierarchy = new CT_PivotHierarchy();
		if (node.Attributes["outline"] != null)
		{
			cT_PivotHierarchy.outline = XmlHelper.ReadBool(node.Attributes["outline"]);
		}
		if (node.Attributes["multipleItemSelectionAllowed"] != null)
		{
			cT_PivotHierarchy.multipleItemSelectionAllowed = XmlHelper.ReadBool(node.Attributes["multipleItemSelectionAllowed"]);
		}
		if (node.Attributes["subtotalTop"] != null)
		{
			cT_PivotHierarchy.subtotalTop = XmlHelper.ReadBool(node.Attributes["subtotalTop"]);
		}
		if (node.Attributes["showInFieldList"] != null)
		{
			cT_PivotHierarchy.showInFieldList = XmlHelper.ReadBool(node.Attributes["showInFieldList"]);
		}
		if (node.Attributes["dragToRow"] != null)
		{
			cT_PivotHierarchy.dragToRow = XmlHelper.ReadBool(node.Attributes["dragToRow"]);
		}
		if (node.Attributes["dragToCol"] != null)
		{
			cT_PivotHierarchy.dragToCol = XmlHelper.ReadBool(node.Attributes["dragToCol"]);
		}
		if (node.Attributes["dragToPage"] != null)
		{
			cT_PivotHierarchy.dragToPage = XmlHelper.ReadBool(node.Attributes["dragToPage"]);
		}
		if (node.Attributes["dragToData"] != null)
		{
			cT_PivotHierarchy.dragToData = XmlHelper.ReadBool(node.Attributes["dragToData"]);
		}
		if (node.Attributes["dragOff"] != null)
		{
			cT_PivotHierarchy.dragOff = XmlHelper.ReadBool(node.Attributes["dragOff"]);
		}
		if (node.Attributes["includeNewItemsInFilter"] != null)
		{
			cT_PivotHierarchy.includeNewItemsInFilter = XmlHelper.ReadBool(node.Attributes["includeNewItemsInFilter"]);
		}
		cT_PivotHierarchy.caption = XmlHelper.ReadString(node.Attributes["caption"]);
		cT_PivotHierarchy.members = new List<CT_Members>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "mps")
			{
				cT_PivotHierarchy.mps = CT_MemberProperties.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_PivotHierarchy.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "members")
			{
				cT_PivotHierarchy.members.Add(CT_Members.Parse(childNode, namespaceManager));
			}
		}
		return cT_PivotHierarchy;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "outline", outline);
		XmlHelper.WriteAttribute(sw, "multipleItemSelectionAllowed", multipleItemSelectionAllowed);
		XmlHelper.WriteAttribute(sw, "subtotalTop", subtotalTop);
		XmlHelper.WriteAttribute(sw, "showInFieldList", showInFieldList);
		XmlHelper.WriteAttribute(sw, "dragToRow", dragToRow);
		XmlHelper.WriteAttribute(sw, "dragToCol", dragToCol);
		XmlHelper.WriteAttribute(sw, "dragToPage", dragToPage);
		XmlHelper.WriteAttribute(sw, "dragToData", dragToData);
		XmlHelper.WriteAttribute(sw, "dragOff", dragOff);
		XmlHelper.WriteAttribute(sw, "includeNewItemsInFilter", includeNewItemsInFilter);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		sw.Write(">");
		if (mps != null)
		{
			mps.Write(sw, "mps");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		if (members != null)
		{
			foreach (CT_Members member in members)
			{
				member.Write(sw, "members");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotHierarchy()
	{
		extLstField = new CT_ExtensionList();
		membersField = new List<CT_Members>();
		mpsField = new CT_MemberProperties();
		outlineField = false;
		multipleItemSelectionAllowedField = false;
		subtotalTopField = false;
		showInFieldListField = true;
		dragToRowField = true;
		dragToColField = true;
		dragToPageField = true;
		dragToDataField = false;
		dragOffField = true;
		includeNewItemsInFilterField = false;
	}
}
