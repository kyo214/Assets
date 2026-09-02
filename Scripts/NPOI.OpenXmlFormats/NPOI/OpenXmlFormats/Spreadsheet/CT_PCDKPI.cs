using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PCDKPI
{
	private string uniqueNameField;

	private string captionField;

	private string displayFolderField;

	private string measureGroupField;

	private string parentField;

	private string valueField;

	private string goalField;

	private string statusField;

	private string trendField;

	private string weightField;

	private string timeField;

	[XmlAttribute]
	public string uniqueName
	{
		get
		{
			return uniqueNameField;
		}
		set
		{
			uniqueNameField = value;
		}
	}

	[XmlAttribute]
	public string caption
	{
		get
		{
			return captionField;
		}
		set
		{
			captionField = value;
		}
	}

	[XmlAttribute]
	public string displayFolder
	{
		get
		{
			return displayFolderField;
		}
		set
		{
			displayFolderField = value;
		}
	}

	[XmlAttribute]
	public string measureGroup
	{
		get
		{
			return measureGroupField;
		}
		set
		{
			measureGroupField = value;
		}
	}

	[XmlAttribute]
	public string parent
	{
		get
		{
			return parentField;
		}
		set
		{
			parentField = value;
		}
	}

	[XmlAttribute]
	public string value
	{
		get
		{
			return valueField;
		}
		set
		{
			valueField = value;
		}
	}

	[XmlAttribute]
	public string goal
	{
		get
		{
			return goalField;
		}
		set
		{
			goalField = value;
		}
	}

	[XmlAttribute]
	public string status
	{
		get
		{
			return statusField;
		}
		set
		{
			statusField = value;
		}
	}

	[XmlAttribute]
	public string trend
	{
		get
		{
			return trendField;
		}
		set
		{
			trendField = value;
		}
	}

	[XmlAttribute]
	public string weight
	{
		get
		{
			return weightField;
		}
		set
		{
			weightField = value;
		}
	}

	[XmlAttribute]
	public string time
	{
		get
		{
			return timeField;
		}
		set
		{
			timeField = value;
		}
	}

	public static CT_PCDKPI Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_PCDKPI
		{
			uniqueName = XmlHelper.ReadString(node.Attributes["uniqueName"]),
			caption = XmlHelper.ReadString(node.Attributes["caption"]),
			displayFolder = XmlHelper.ReadString(node.Attributes["displayFolder"]),
			measureGroup = XmlHelper.ReadString(node.Attributes["measureGroup"]),
			parent = XmlHelper.ReadString(node.Attributes["parent"]),
			value = XmlHelper.ReadString(node.Attributes["value"]),
			goal = XmlHelper.ReadString(node.Attributes["goal"]),
			status = XmlHelper.ReadString(node.Attributes["status"]),
			trend = XmlHelper.ReadString(node.Attributes["trend"]),
			weight = XmlHelper.ReadString(node.Attributes["weight"]),
			time = XmlHelper.ReadString(node.Attributes["time"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "uniqueName", uniqueName);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		XmlHelper.WriteAttribute(sw, "displayFolder", displayFolder);
		XmlHelper.WriteAttribute(sw, "measureGroup", measureGroup);
		XmlHelper.WriteAttribute(sw, "parent", parent);
		XmlHelper.WriteAttribute(sw, "value", value);
		XmlHelper.WriteAttribute(sw, "goal", goal);
		XmlHelper.WriteAttribute(sw, "status", status);
		XmlHelper.WriteAttribute(sw, "trend", trend);
		XmlHelper.WriteAttribute(sw, "weight", weight);
		XmlHelper.WriteAttribute(sw, "time", time);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
