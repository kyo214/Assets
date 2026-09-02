using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Format
{
	private CT_PivotArea pivotAreaField;

	private CT_ExtensionList extLstField;

	private ST_FormatAction actionField;

	private uint dxfIdField;

	private bool dxfIdFieldSpecified;

	[XmlElement(Order = 0)]
	public CT_PivotArea pivotArea
	{
		get
		{
			return pivotAreaField;
		}
		set
		{
			pivotAreaField = value;
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
	[DefaultValue(ST_FormatAction.formatting)]
	public ST_FormatAction action
	{
		get
		{
			return actionField;
		}
		set
		{
			actionField = value;
		}
	}

	[XmlAttribute]
	public uint dxfId
	{
		get
		{
			return dxfIdField;
		}
		set
		{
			dxfIdField = value;
		}
	}

	[XmlIgnore]
	public bool dxfIdSpecified
	{
		get
		{
			return dxfIdFieldSpecified;
		}
		set
		{
			dxfIdFieldSpecified = value;
		}
	}

	public static CT_Format Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Format cT_Format = new CT_Format();
		if (node.Attributes["action"] != null)
		{
			cT_Format.action = (ST_FormatAction)Enum.Parse(typeof(ST_FormatAction), node.Attributes["action"].Value);
		}
		if (node.Attributes["dxfId"] != null)
		{
			cT_Format.dxfId = XmlHelper.ReadUInt(node.Attributes["dxfId"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotArea")
			{
				cT_Format.pivotArea = CT_PivotArea.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_Format.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_Format;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "action", action.ToString());
		XmlHelper.WriteAttribute(sw, "dxfId", dxfId);
		sw.Write(">");
		if (pivotArea != null)
		{
			pivotArea.Write(sw, "pivotArea");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Format()
	{
		extLstField = new CT_ExtensionList();
		pivotAreaField = new CT_PivotArea();
		actionField = ST_FormatAction.formatting;
	}
}
