using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CacheSource
{
	private object itemField;

	private ST_SourceType typeField;

	private uint connectionIdField;

	private CT_WorksheetSource worksheetSourceField;

	private CT_ExtensionList extLstField;

	private CT_Consolidation consolidationField;

	[XmlElement("consolidation", typeof(CT_Consolidation), Order = 0)]
	[XmlElement("extLst", typeof(CT_ExtensionList), Order = 0)]
	[XmlElement("worksheetSource", typeof(CT_WorksheetSource), Order = 0)]
	public object Item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}

	[XmlAttribute]
	public ST_SourceType type
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
	[DefaultValue(typeof(uint), "0")]
	public uint connectionId
	{
		get
		{
			return connectionIdField;
		}
		set
		{
			connectionIdField = value;
		}
	}

	public CT_WorksheetSource worksheetSource
	{
		get
		{
			return worksheetSourceField;
		}
		set
		{
			worksheetSourceField = value;
		}
	}

	public CT_Consolidation consolidation
	{
		get
		{
			return consolidationField;
		}
		set
		{
			consolidationField = value;
		}
	}

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

	public static CT_CacheSource Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CacheSource cT_CacheSource = new CT_CacheSource();
		if (node.Attributes["type"] != null)
		{
			cT_CacheSource.type = (ST_SourceType)Enum.Parse(typeof(ST_SourceType), node.Attributes["type"].Value);
		}
		if (node.Attributes["connectionId"] != null)
		{
			cT_CacheSource.connectionId = XmlHelper.ReadUInt(node.Attributes["connectionId"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "worksheetSource")
			{
				cT_CacheSource.worksheetSource = CT_WorksheetSource.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "consolidation")
			{
				cT_CacheSource.consolidation = CT_Consolidation.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_CacheSource.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_CacheSource;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "type", type.ToString());
		XmlHelper.WriteAttribute(sw, "connectionId", connectionId);
		sw.Write(">");
		if (worksheetSource != null)
		{
			worksheetSource.Write(sw, "worksheetSource");
		}
		if (consolidation != null)
		{
			consolidation.Write(sw, "consolidation");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CacheSource()
	{
		connectionIdField = 0u;
	}

	public CT_WorksheetSource AddNewWorksheetSource()
	{
		worksheetSourceField = new CT_WorksheetSource();
		return worksheetSourceField;
	}
}
