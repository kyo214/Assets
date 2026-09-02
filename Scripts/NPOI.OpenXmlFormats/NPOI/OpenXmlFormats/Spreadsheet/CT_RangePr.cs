using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_RangePr
{
	private bool autoStartField;

	private bool autoEndField;

	private ST_GroupBy groupByField;

	private double startNumField;

	private bool startNumFieldSpecified;

	private double endNumField;

	private bool endNumFieldSpecified;

	private DateTime? startDateField;

	private bool startDateFieldSpecified;

	private DateTime? endDateField;

	private bool endDateFieldSpecified;

	private double groupIntervalField;

	[XmlAttribute]
	[DefaultValue(true)]
	public bool autoStart
	{
		get
		{
			return autoStartField;
		}
		set
		{
			autoStartField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool autoEnd
	{
		get
		{
			return autoEndField;
		}
		set
		{
			autoEndField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_GroupBy.range)]
	public ST_GroupBy groupBy
	{
		get
		{
			return groupByField;
		}
		set
		{
			groupByField = value;
		}
	}

	[XmlAttribute]
	public double startNum
	{
		get
		{
			return startNumField;
		}
		set
		{
			startNumField = value;
		}
	}

	[XmlIgnore]
	public bool startNumSpecified
	{
		get
		{
			return startNumFieldSpecified;
		}
		set
		{
			startNumFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public double endNum
	{
		get
		{
			return endNumField;
		}
		set
		{
			endNumField = value;
		}
	}

	[XmlIgnore]
	public bool endNumSpecified
	{
		get
		{
			return endNumFieldSpecified;
		}
		set
		{
			endNumFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public DateTime? startDate
	{
		get
		{
			return startDateField;
		}
		set
		{
			startDateField = value;
		}
	}

	[XmlIgnore]
	public bool startDateSpecified
	{
		get
		{
			return startDateFieldSpecified;
		}
		set
		{
			startDateFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public DateTime? endDate
	{
		get
		{
			return endDateField;
		}
		set
		{
			endDateField = value;
		}
	}

	[XmlIgnore]
	public bool endDateSpecified
	{
		get
		{
			return endDateFieldSpecified;
		}
		set
		{
			endDateFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(1.0)]
	public double groupInterval
	{
		get
		{
			return groupIntervalField;
		}
		set
		{
			groupIntervalField = value;
		}
	}

	public static CT_RangePr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RangePr cT_RangePr = new CT_RangePr();
		if (node.Attributes["autoStart"] != null)
		{
			cT_RangePr.autoStart = XmlHelper.ReadBool(node.Attributes["autoStart"]);
		}
		if (node.Attributes["autoEnd"] != null)
		{
			cT_RangePr.autoEnd = XmlHelper.ReadBool(node.Attributes["autoEnd"]);
		}
		if (node.Attributes["groupBy"] != null)
		{
			cT_RangePr.groupBy = (ST_GroupBy)Enum.Parse(typeof(ST_GroupBy), node.Attributes["groupBy"].Value);
		}
		if (node.Attributes["startNum"] != null)
		{
			cT_RangePr.startNum = XmlHelper.ReadDouble(node.Attributes["startNum"]);
		}
		if (node.Attributes["endNum"] != null)
		{
			cT_RangePr.endNum = XmlHelper.ReadDouble(node.Attributes["endNum"]);
		}
		if (node.Attributes["startDate"] != null)
		{
			cT_RangePr.startDate = XmlHelper.ReadDateTime(node.Attributes["startDate"]);
		}
		if (node.Attributes["endDate"] != null)
		{
			cT_RangePr.endDate = XmlHelper.ReadDateTime(node.Attributes["endDate"]);
		}
		if (node.Attributes["groupInterval"] != null)
		{
			cT_RangePr.groupInterval = XmlHelper.ReadDouble(node.Attributes["groupInterval"]);
		}
		return cT_RangePr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "autoStart", autoStart);
		XmlHelper.WriteAttribute(sw, "autoEnd", autoEnd);
		XmlHelper.WriteAttribute(sw, "groupBy", groupBy.ToString());
		XmlHelper.WriteAttribute(sw, "startNum", startNum);
		XmlHelper.WriteAttribute(sw, "endNum", endNum);
		XmlHelper.WriteAttribute(sw, "startDate", startDate);
		XmlHelper.WriteAttribute(sw, "endDate", endDate);
		XmlHelper.WriteAttribute(sw, "groupInterval", groupInterval);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_RangePr()
	{
		autoStartField = true;
		autoEndField = true;
		groupByField = ST_GroupBy.range;
		groupIntervalField = 1.0;
	}
}
