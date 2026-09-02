using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_DataBar
{
	private List<CT_Cfvo> cfvoField;

	private CT_Color colorField;

	private uint minLengthField;

	private uint maxLengthField;

	private bool showValueField;

	public List<CT_Cfvo> cfvo
	{
		get
		{
			return cfvoField;
		}
		set
		{
			cfvoField = value;
		}
	}

	public CT_Color color
	{
		get
		{
			return colorField;
		}
		set
		{
			colorField = value;
		}
	}

	[DefaultValue(typeof(uint), "10")]
	public uint minLength
	{
		get
		{
			return minLengthField;
		}
		set
		{
			minLengthField = value;
		}
	}

	[DefaultValue(typeof(uint), "90")]
	public uint maxLength
	{
		get
		{
			return maxLengthField;
		}
		set
		{
			maxLengthField = value;
		}
	}

	[DefaultValue(true)]
	public bool showValue
	{
		get
		{
			return showValueField;
		}
		set
		{
			showValueField = value;
		}
	}

	public CT_DataBar()
	{
		minLengthField = 10u;
		maxLengthField = 90u;
		showValueField = true;
	}

	public static CT_DataBar Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_DataBar cT_DataBar = new CT_DataBar();
		cT_DataBar.minLength = XmlHelper.ReadUInt(node.Attributes["minLength"]);
		cT_DataBar.maxLength = XmlHelper.ReadUInt(node.Attributes["maxLength"]);
		if (node.Attributes["showValue"] == null)
		{
			cT_DataBar.showValue = true;
		}
		else
		{
			cT_DataBar.showValue = XmlHelper.ReadBool(node.Attributes["showValue"]);
		}
		cT_DataBar.cfvo = new List<CT_Cfvo>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "color")
			{
				cT_DataBar.color = CT_Color.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "cfvo")
			{
				cT_DataBar.cfvo.Add(CT_Cfvo.Parse(childNode, namespaceManager));
			}
		}
		return cT_DataBar;
	}

	public CT_Cfvo AddNewCfvo()
	{
		CT_Cfvo cT_Cfvo = new CT_Cfvo();
		if (cfvo == null)
		{
			cfvo = new List<CT_Cfvo>();
		}
		cfvo.Add(cT_Cfvo);
		return cT_Cfvo;
	}

	public bool IsSetShowValue()
	{
		return showValueField;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "minLength", minLength);
		XmlHelper.WriteAttribute(sw, "maxLength", maxLength);
		if (showValue)
		{
			XmlHelper.WriteAttribute(sw, "showValue", showValue);
		}
		sw.Write(">");
		if (cfvo != null)
		{
			foreach (CT_Cfvo item in cfvo)
			{
				item.Write(sw, "cfvo");
			}
		}
		if (color != null)
		{
			color.Write(sw, "color");
		}
		sw.Write($"</{nodeName}>");
	}
}
