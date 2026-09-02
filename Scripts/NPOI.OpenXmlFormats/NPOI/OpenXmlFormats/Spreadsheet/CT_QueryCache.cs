using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_QueryCache
{
	private List<CT_Query> queryField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("query", Order = 0)]
	public List<CT_Query> query
	{
		get
		{
			return queryField;
		}
		set
		{
			queryField = value;
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

	public static CT_QueryCache Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_QueryCache cT_QueryCache = new CT_QueryCache();
		if (node.Attributes["count"] != null)
		{
			cT_QueryCache.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_QueryCache.query = new List<CT_Query>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "query")
			{
				cT_QueryCache.query.Add(CT_Query.Parse(childNode, namespaceManager));
			}
		}
		return cT_QueryCache;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (query != null)
		{
			foreach (CT_Query item in query)
			{
				item.Write(sw, "query");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_QueryCache()
	{
		queryField = new List<CT_Query>();
	}
}
