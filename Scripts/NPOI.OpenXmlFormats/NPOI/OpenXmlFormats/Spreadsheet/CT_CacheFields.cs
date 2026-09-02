using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CacheFields
{
	private List<CT_CacheField> cacheFieldField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("cacheField", Order = 0)]
	public List<CT_CacheField> cacheField
	{
		get
		{
			return cacheFieldField;
		}
		set
		{
			cacheFieldField = value;
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

	public static CT_CacheFields Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CacheFields cT_CacheFields = new CT_CacheFields();
		if (node.Attributes["count"] != null)
		{
			cT_CacheFields.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_CacheFields.cacheField = new List<CT_CacheField>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "cacheField")
			{
				cT_CacheFields.cacheField.Add(CT_CacheField.Parse(childNode, namespaceManager));
			}
		}
		return cT_CacheFields;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (cacheField != null)
		{
			foreach (CT_CacheField item in cacheField)
			{
				item.Write(sw, "cacheField");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CacheFields()
	{
		cacheFieldField = new List<CT_CacheField>();
	}

	public CT_CacheField AddNewCacheField()
	{
		CT_CacheField cT_CacheField = new CT_CacheField();
		cacheFieldField.Add(cT_CacheField);
		return cT_CacheField;
	}

	public uint SizeOfCacheFieldArray()
	{
		if (cacheFieldField == null)
		{
			return 0u;
		}
		return (uint)cacheFieldField.Count;
	}
}
