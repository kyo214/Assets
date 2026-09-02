using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_TupleCache
{
	private CT_PCDSDTCEntries entriesField;

	private CT_Sets setsField;

	private CT_QueryCache queryCacheField;

	private CT_ServerFormats serverFormatsField;

	private CT_ExtensionList extLstField;

	[XmlElement(Order = 0)]
	public CT_PCDSDTCEntries entries
	{
		get
		{
			return entriesField;
		}
		set
		{
			entriesField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_Sets sets
	{
		get
		{
			return setsField;
		}
		set
		{
			setsField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_QueryCache queryCache
	{
		get
		{
			return queryCacheField;
		}
		set
		{
			queryCacheField = value;
		}
	}

	[XmlElement(Order = 3)]
	public CT_ServerFormats serverFormats
	{
		get
		{
			return serverFormatsField;
		}
		set
		{
			serverFormatsField = value;
		}
	}

	[XmlElement(Order = 4)]
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

	public static CT_TupleCache Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TupleCache cT_TupleCache = new CT_TupleCache();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "entries")
			{
				cT_TupleCache.entries = CT_PCDSDTCEntries.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "sets")
			{
				cT_TupleCache.sets = CT_Sets.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "queryCache")
			{
				cT_TupleCache.queryCache = CT_QueryCache.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "serverFormats")
			{
				cT_TupleCache.serverFormats = CT_ServerFormats.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_TupleCache.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_TupleCache;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		sw.Write(">");
		if (entries != null)
		{
			entries.Write(sw, "entries");
		}
		if (sets != null)
		{
			sets.Write(sw, "sets");
		}
		if (queryCache != null)
		{
			queryCache.Write(sw, "queryCache");
		}
		if (serverFormats != null)
		{
			serverFormats.Write(sw, "serverFormats");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_TupleCache()
	{
		extLstField = new CT_ExtensionList();
		serverFormatsField = new CT_ServerFormats();
		queryCacheField = new CT_QueryCache();
		setsField = new CT_Sets();
		entriesField = new CT_PCDSDTCEntries();
	}
}
