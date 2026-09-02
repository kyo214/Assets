using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_DataField
{
	private CT_ExtensionList extLstField;

	private string nameField;

	private uint fldField;

	private ST_DataConsolidateFunction subtotalField;

	private ST_ShowDataAs showDataAsField;

	private int baseFieldField;

	private uint baseItemField;

	private uint numFmtIdField;

	private bool numFmtIdFieldSpecified;

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
	public uint fld
	{
		get
		{
			return fldField;
		}
		set
		{
			fldField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_DataConsolidateFunction.sum)]
	public ST_DataConsolidateFunction subtotal
	{
		get
		{
			return subtotalField;
		}
		set
		{
			subtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_ShowDataAs.normal)]
	public ST_ShowDataAs showDataAs
	{
		get
		{
			return showDataAsField;
		}
		set
		{
			showDataAsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(-1)]
	public int baseField
	{
		get
		{
			return baseFieldField;
		}
		set
		{
			baseFieldField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "1048832")]
	public uint baseItem
	{
		get
		{
			return baseItemField;
		}
		set
		{
			baseItemField = value;
		}
	}

	[XmlAttribute]
	public uint numFmtId
	{
		get
		{
			return numFmtIdField;
		}
		set
		{
			numFmtIdField = value;
		}
	}

	[XmlIgnore]
	public bool numFmtIdSpecified
	{
		get
		{
			return numFmtIdFieldSpecified;
		}
		set
		{
			numFmtIdFieldSpecified = value;
		}
	}

	public static CT_DataField Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_DataField cT_DataField = new CT_DataField();
		cT_DataField.name = XmlHelper.ReadString(node.Attributes["name"]);
		if (node.Attributes["fld"] != null)
		{
			cT_DataField.fld = XmlHelper.ReadUInt(node.Attributes["fld"]);
		}
		if (node.Attributes["subtotal"] != null)
		{
			cT_DataField.subtotal = (ST_DataConsolidateFunction)Enum.Parse(typeof(ST_DataConsolidateFunction), node.Attributes["subtotal"].Value);
		}
		if (node.Attributes["showDataAs"] != null)
		{
			cT_DataField.showDataAs = (ST_ShowDataAs)Enum.Parse(typeof(ST_ShowDataAs), node.Attributes["showDataAs"].Value);
		}
		if (node.Attributes["baseField"] != null)
		{
			cT_DataField.baseField = XmlHelper.ReadInt(node.Attributes["baseField"]);
		}
		if (node.Attributes["baseItem"] != null)
		{
			cT_DataField.baseItem = XmlHelper.ReadUInt(node.Attributes["baseItem"]);
		}
		if (node.Attributes["numFmtId"] != null)
		{
			cT_DataField.numFmtId = XmlHelper.ReadUInt(node.Attributes["numFmtId"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_DataField.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_DataField;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "fld", fld);
		XmlHelper.WriteAttribute(sw, "subtotal", subtotal.ToString());
		XmlHelper.WriteAttribute(sw, "showDataAs", showDataAs.ToString());
		XmlHelper.WriteAttribute(sw, "baseField", baseField);
		XmlHelper.WriteAttribute(sw, "baseItem", baseItem);
		XmlHelper.WriteAttribute(sw, "numFmtId", numFmtId);
		sw.Write(">");
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_DataField()
	{
		extLstField = new CT_ExtensionList();
		subtotalField = ST_DataConsolidateFunction.sum;
		showDataAsField = ST_ShowDataAs.normal;
		baseFieldField = -1;
		baseItemField = 1048832u;
	}
}
