using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Set
{
	private List<CT_Tuples> tplsField;

	private CT_Tuples sortByTupleField;

	private uint countField;

	private bool countFieldSpecified;

	private int maxRankField;

	private string setDefinitionField;

	private ST_SortType sortTypeField;

	private bool queryFailedField;

	[XmlElement("tpls", Order = 0)]
	public List<CT_Tuples> tpls
	{
		get
		{
			return tplsField;
		}
		set
		{
			tplsField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_Tuples sortByTuple
	{
		get
		{
			return sortByTupleField;
		}
		set
		{
			sortByTupleField = value;
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

	[XmlAttribute]
	public int maxRank
	{
		get
		{
			return maxRankField;
		}
		set
		{
			maxRankField = value;
		}
	}

	[XmlAttribute]
	public string setDefinition
	{
		get
		{
			return setDefinitionField;
		}
		set
		{
			setDefinitionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_SortType.none)]
	public ST_SortType sortType
	{
		get
		{
			return sortTypeField;
		}
		set
		{
			sortTypeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool queryFailed
	{
		get
		{
			return queryFailedField;
		}
		set
		{
			queryFailedField = value;
		}
	}

	public static CT_Set Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Set cT_Set = new CT_Set();
		if (node.Attributes["count"] != null)
		{
			cT_Set.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		if (node.Attributes["maxRank"] != null)
		{
			cT_Set.maxRank = XmlHelper.ReadInt(node.Attributes["maxRank"]);
		}
		cT_Set.setDefinition = XmlHelper.ReadString(node.Attributes["setDefinition"]);
		if (node.Attributes["sortType"] != null)
		{
			cT_Set.sortType = (ST_SortType)Enum.Parse(typeof(ST_SortType), node.Attributes["sortType"].Value);
		}
		if (node.Attributes["queryFailed"] != null)
		{
			cT_Set.queryFailed = XmlHelper.ReadBool(node.Attributes["queryFailed"]);
		}
		cT_Set.tpls = new List<CT_Tuples>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "sortByTuple")
			{
				cT_Set.sortByTuple = CT_Tuples.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "tpls")
			{
				cT_Set.tpls.Add(CT_Tuples.Parse(childNode, namespaceManager));
			}
		}
		return cT_Set;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		XmlHelper.WriteAttribute(sw, "maxRank", maxRank);
		XmlHelper.WriteAttribute(sw, "setDefinition", setDefinition);
		XmlHelper.WriteAttribute(sw, "sortType", sortType.ToString());
		XmlHelper.WriteAttribute(sw, "queryFailed", queryFailed);
		sw.Write(">");
		if (sortByTuple != null)
		{
			sortByTuple.Write(sw, "sortByTuple");
		}
		if (tpls != null)
		{
			foreach (CT_Tuples tpl in tpls)
			{
				tpl.Write(sw, "tpls");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Set()
	{
		sortByTupleField = new CT_Tuples();
		tplsField = new List<CT_Tuples>();
		sortTypeField = ST_SortType.none;
		queryFailedField = false;
	}
}
