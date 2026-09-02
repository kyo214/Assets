using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot("pivotCacheRecords", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public class CT_PivotCacheRecords
{
	private List<object> rField;

	private CT_ExtensionList extLstField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlArray(Order = 0)]
	[XmlArrayItem("b", typeof(CT_Boolean), IsNullable = false)]
	[XmlArrayItem("d", typeof(CT_DateTime), IsNullable = false)]
	[XmlArrayItem("e", typeof(CT_Error), IsNullable = false)]
	[XmlArrayItem("m", typeof(CT_Missing), IsNullable = false)]
	[XmlArrayItem("n", typeof(CT_Number), IsNullable = false)]
	[XmlArrayItem("s", typeof(CT_String), IsNullable = false)]
	[XmlArrayItem("x", typeof(CT_Index), IsNullable = false)]
	public List<object> r
	{
		get
		{
			return rField;
		}
		set
		{
			rField = value;
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

	public static CT_PivotCacheRecords Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotCacheRecords cT_PivotCacheRecords = new CT_PivotCacheRecords();
		if (node.Attributes["count"] != null)
		{
			cT_PivotCacheRecords.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PivotCacheRecords.r = new List<object>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_PivotCacheRecords.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "n")
			{
				cT_PivotCacheRecords.r.Add(CT_Number.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "b")
			{
				cT_PivotCacheRecords.r.Add(CT_Boolean.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "d")
			{
				cT_PivotCacheRecords.r.Add(CT_DateTime.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "e")
			{
				cT_PivotCacheRecords.r.Add(CT_Error.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "m")
			{
				cT_PivotCacheRecords.r.Add(CT_Missing.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "s")
			{
				cT_PivotCacheRecords.r.Add(CT_String.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "x")
			{
				cT_PivotCacheRecords.r.Add(CT_Index.Parse(childNode, namespaceManager));
			}
		}
		return cT_PivotCacheRecords;
	}

	internal void Write(StreamWriter sw)
	{
		sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
		sw.Write("<pivotCacheRecords xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" ");
		sw.Write("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" ");
		sw.Write("xmlns:s=\"http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes\" ");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		foreach (object item in r)
		{
			if (item is CT_Number)
			{
				((CT_Number)item).Write(sw, "n");
			}
			else if (item is CT_Boolean)
			{
				((CT_Boolean)item).Write(sw, "b");
			}
			else if (item is CT_DateTime)
			{
				((CT_DateTime)item).Write(sw, "d");
			}
			else if (item is CT_Error)
			{
				((CT_Error)item).Write(sw, "e");
			}
			else if (item is CT_Missing)
			{
				((CT_Missing)item).Write(sw, "m");
			}
			else if (item is CT_String)
			{
				((CT_String)item).Write(sw, "s");
			}
			else if (item is CT_Index)
			{
				((CT_Index)item).Write(sw, "x");
			}
		}
		sw.Write($"</pivotCacheRecords>");
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		Write(sw);
	}

	public CT_PivotCacheRecords()
	{
		extLstField = new CT_ExtensionList();
		rField = new List<object>();
	}
}
