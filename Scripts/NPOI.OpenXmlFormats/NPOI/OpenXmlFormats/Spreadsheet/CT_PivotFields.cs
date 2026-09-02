using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotFields
{
	private List<CT_PivotField> pivotFieldField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("pivotField", Order = 0)]
	public List<CT_PivotField> pivotField
	{
		get
		{
			return pivotFieldField;
		}
		set
		{
			pivotFieldField = value;
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

	public static CT_PivotFields Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotFields cT_PivotFields = new CT_PivotFields();
		if (node.Attributes["count"] != null)
		{
			cT_PivotFields.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PivotFields.pivotField = new List<CT_PivotField>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotField")
			{
				cT_PivotFields.pivotField.Add(CT_PivotField.Parse(childNode, namespaceManager));
			}
		}
		return cT_PivotFields;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (pivotField != null)
		{
			foreach (CT_PivotField item in pivotField)
			{
				item.Write(sw, "pivotField");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotFields()
	{
		pivotFieldField = new List<CT_PivotField>();
	}

	public void SetPivotFieldArray(int columnIndex, CT_PivotField pivotField)
	{
		pivotFieldField[columnIndex] = pivotField;
	}

	public CT_PivotField AddNewPivotField()
	{
		if (pivotFieldField == null)
		{
			pivotFieldField = new List<CT_PivotField>();
		}
		CT_PivotField cT_PivotField = new CT_PivotField();
		pivotFieldField.Add(cT_PivotField);
		return cT_PivotField;
	}

	public uint SizeOfPivotFieldArray()
	{
		if (pivotFieldField == null)
		{
			pivotFieldField = new List<CT_PivotField>();
		}
		return (uint)pivotFieldField.Count;
	}

	public CT_PivotField GetPivotFieldArray(int columnIndex)
	{
		if (pivotFieldField == null)
		{
			pivotFieldField = new List<CT_PivotField>();
		}
		return pivotFieldField[columnIndex];
	}
}
