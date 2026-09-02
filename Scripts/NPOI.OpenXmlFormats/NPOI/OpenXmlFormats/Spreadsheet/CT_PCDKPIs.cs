using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PCDKPIs
{
	private List<CT_PCDKPI> kpiField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("kpi", Order = 0)]
	public List<CT_PCDKPI> kpi
	{
		get
		{
			return kpiField;
		}
		set
		{
			kpiField = value;
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

	public static CT_PCDKPIs Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PCDKPIs cT_PCDKPIs = new CT_PCDKPIs();
		if (node.Attributes["count"] != null)
		{
			cT_PCDKPIs.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PCDKPIs.kpi = new List<CT_PCDKPI>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "kpi")
			{
				cT_PCDKPIs.kpi.Add(CT_PCDKPI.Parse(childNode, namespaceManager));
			}
		}
		return cT_PCDKPIs;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (kpi != null)
		{
			foreach (CT_PCDKPI item in kpi)
			{
				item.Write(sw, "kpi");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PCDKPIs()
	{
		kpiField = new List<CT_PCDKPI>();
	}
}
