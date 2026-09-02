using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_Columns
{
	private List<CT_Column> colField;

	private ST_OnOff equalWidthField;

	private bool equalWidthFieldSpecified;

	private ulong spaceField;

	private bool spaceFieldSpecified;

	private string numField;

	private ST_OnOff sepField;

	private bool sepFieldSpecified;

	[XmlElement("col", Order = 0)]
	public List<CT_Column> col
	{
		get
		{
			return colField;
		}
		set
		{
			colField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff equalWidth
	{
		get
		{
			return equalWidthField;
		}
		set
		{
			equalWidthField = value;
		}
	}

	[XmlIgnore]
	public bool equalWidthSpecified
	{
		get
		{
			return equalWidthFieldSpecified;
		}
		set
		{
			equalWidthFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong space
	{
		get
		{
			return spaceField;
		}
		set
		{
			spaceField = value;
		}
	}

	[XmlIgnore]
	public bool spaceSpecified
	{
		get
		{
			return spaceFieldSpecified;
		}
		set
		{
			spaceFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "integer")]
	public string num
	{
		get
		{
			return numField;
		}
		set
		{
			numField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff sep
	{
		get
		{
			return sepField;
		}
		set
		{
			sepField = value;
		}
	}

	[XmlIgnore]
	public bool sepSpecified
	{
		get
		{
			return sepFieldSpecified;
		}
		set
		{
			sepFieldSpecified = value;
		}
	}

	public CT_Columns()
	{
		equalWidthField = ST_OnOff.off;
		sepField = ST_OnOff.off;
	}

	public static CT_Columns Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Columns cT_Columns = new CT_Columns();
		if (node.Attributes["w:equalWidth"] != null)
		{
			cT_Columns.equalWidth = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:equalWidth"].Value, ignoreCase: true);
		}
		cT_Columns.space = XmlHelper.ReadULong(node.Attributes["w:space"]);
		cT_Columns.num = XmlHelper.ReadString(node.Attributes["w:num"]);
		if (node.Attributes["w:sep"] != null)
		{
			cT_Columns.sep = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:sep"].Value, ignoreCase: true);
		}
		cT_Columns.col = new List<CT_Column>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "col")
			{
				cT_Columns.col.Add(CT_Column.Parse(childNode, namespaceManager));
			}
		}
		return cT_Columns;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		if (equalWidth != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:equalWidth", equalWidth.ToString());
		}
		XmlHelper.WriteAttribute(sw, "w:space", space);
		XmlHelper.WriteAttribute(sw, "w:num", num);
		if (sep != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:sep", sep.ToString());
		}
		if (col != null && col.Count > 0)
		{
			sw.Write(">");
			foreach (CT_Column item in col)
			{
				item.Write(sw, "col");
			}
			sw.Write($"</w:{nodeName}>");
		}
		else
		{
			sw.Write("/>");
		}
	}
}
