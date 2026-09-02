using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_FieldGroup
{
	private CT_RangePr rangePrField;

	private CT_DiscretePr discretePrField;

	private CT_GroupItems groupItemsField;

	private uint parField;

	private bool parFieldSpecified;

	private uint baseField;

	private bool baseFieldSpecified;

	[XmlElement(Order = 0)]
	public CT_RangePr rangePr
	{
		get
		{
			return rangePrField;
		}
		set
		{
			rangePrField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_DiscretePr discretePr
	{
		get
		{
			return discretePrField;
		}
		set
		{
			discretePrField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_GroupItems groupItems
	{
		get
		{
			return groupItemsField;
		}
		set
		{
			groupItemsField = value;
		}
	}

	[XmlAttribute]
	public uint par
	{
		get
		{
			return parField;
		}
		set
		{
			parField = value;
		}
	}

	[XmlIgnore]
	public bool parSpecified
	{
		get
		{
			return parFieldSpecified;
		}
		set
		{
			parFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint @base
	{
		get
		{
			return baseField;
		}
		set
		{
			baseField = value;
		}
	}

	[XmlIgnore]
	public bool baseSpecified
	{
		get
		{
			return baseFieldSpecified;
		}
		set
		{
			baseFieldSpecified = value;
		}
	}

	public static CT_FieldGroup Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FieldGroup cT_FieldGroup = new CT_FieldGroup();
		if (node.Attributes["par"] != null)
		{
			cT_FieldGroup.par = XmlHelper.ReadUInt(node.Attributes["par"]);
		}
		if (node.Attributes["base"] != null)
		{
			cT_FieldGroup.@base = XmlHelper.ReadUInt(node.Attributes["base"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "rangePr")
			{
				cT_FieldGroup.rangePr = CT_RangePr.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "discretePr")
			{
				cT_FieldGroup.discretePr = CT_DiscretePr.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "groupItems")
			{
				cT_FieldGroup.groupItems = CT_GroupItems.Parse(childNode, namespaceManager);
			}
		}
		return cT_FieldGroup;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "par", par);
		XmlHelper.WriteAttribute(sw, "base", @base);
		sw.Write(">");
		if (rangePr != null)
		{
			rangePr.Write(sw, "rangePr");
		}
		if (discretePr != null)
		{
			discretePr.Write(sw, "discretePr");
		}
		if (groupItems != null)
		{
			groupItems.Write(sw, "groupItems");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_FieldGroup()
	{
		groupItemsField = new CT_GroupItems();
		discretePrField = new CT_DiscretePr();
		rangePrField = new CT_RangePr();
	}
}
