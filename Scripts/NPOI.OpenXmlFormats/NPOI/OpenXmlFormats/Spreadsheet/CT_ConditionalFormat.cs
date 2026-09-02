using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ConditionalFormat
{
	private CT_PivotAreas pivotAreasField;

	private CT_ExtensionList extLstField;

	private ST_Scope scopeField;

	private ST_Type typeField;

	private uint priorityField;

	[XmlElement(Order = 0)]
	public CT_PivotAreas pivotAreas
	{
		get
		{
			return pivotAreasField;
		}
		set
		{
			pivotAreasField = value;
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
	[DefaultValue(ST_Scope.selection)]
	public ST_Scope scope
	{
		get
		{
			return scopeField;
		}
		set
		{
			scopeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_Type.none)]
	public ST_Type type
	{
		get
		{
			return typeField;
		}
		set
		{
			typeField = value;
		}
	}

	[XmlAttribute]
	public uint priority
	{
		get
		{
			return priorityField;
		}
		set
		{
			priorityField = value;
		}
	}

	public static CT_ConditionalFormat Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ConditionalFormat cT_ConditionalFormat = new CT_ConditionalFormat();
		if (node.Attributes["scope"] != null)
		{
			cT_ConditionalFormat.scope = (ST_Scope)Enum.Parse(typeof(ST_Scope), node.Attributes["scope"].Value);
		}
		if (node.Attributes["type"] != null)
		{
			cT_ConditionalFormat.type = (ST_Type)Enum.Parse(typeof(ST_Type), node.Attributes["type"].Value);
		}
		if (node.Attributes["priority"] != null)
		{
			cT_ConditionalFormat.priority = XmlHelper.ReadUInt(node.Attributes["priority"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotAreas")
			{
				cT_ConditionalFormat.pivotAreas = CT_PivotAreas.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_ConditionalFormat.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_ConditionalFormat;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "scope", scope.ToString());
		XmlHelper.WriteAttribute(sw, "type", type.ToString());
		XmlHelper.WriteAttribute(sw, "priority", priority);
		sw.Write(">");
		if (pivotAreas != null)
		{
			pivotAreas.Write(sw, "pivotAreas");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ConditionalFormat()
	{
		extLstField = new CT_ExtensionList();
		pivotAreasField = new CT_PivotAreas();
		scopeField = ST_Scope.selection;
		typeField = ST_Type.none;
	}
}
