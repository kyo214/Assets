using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CalculatedItem
{
	private CT_PivotArea pivotAreaField;

	private CT_ExtensionList extLstField;

	private uint fieldField;

	private bool fieldFieldSpecified;

	private string formulaField;

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
	public uint field
	{
		get
		{
			return fieldField;
		}
		set
		{
			fieldField = value;
		}
	}

	[XmlIgnore]
	public bool fieldSpecified
	{
		get
		{
			return fieldFieldSpecified;
		}
		set
		{
			fieldFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string formula
	{
		get
		{
			return formulaField;
		}
		set
		{
			formulaField = value;
		}
	}

	public static CT_CalculatedItem Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CalculatedItem cT_CalculatedItem = new CT_CalculatedItem();
		if (node.Attributes["field"] != null)
		{
			cT_CalculatedItem.field = XmlHelper.ReadUInt(node.Attributes["field"]);
		}
		cT_CalculatedItem.formula = XmlHelper.ReadString(node.Attributes["formula"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotArea")
			{
				cT_CalculatedItem.pivotArea = CT_PivotArea.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_CalculatedItem.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_CalculatedItem;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "field", field);
		XmlHelper.WriteAttribute(sw, "formula", formula);
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

	public CT_CalculatedItem()
	{
		extLstField = new CT_ExtensionList();
		pivotAreaField = new CT_PivotArea();
	}
}
