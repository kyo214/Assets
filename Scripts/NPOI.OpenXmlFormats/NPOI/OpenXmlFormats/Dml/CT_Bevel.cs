using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Bevel
{
	private long wField;

	private long hField;

	private ST_BevelPresetType prstField;

	[XmlAttribute]
	[DefaultValue(typeof(long), "76200")]
	public long w
	{
		get
		{
			return wField;
		}
		set
		{
			wField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(long), "76200")]
	public long h
	{
		get
		{
			return hField;
		}
		set
		{
			hField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_BevelPresetType.circle)]
	public ST_BevelPresetType prst
	{
		get
		{
			return prstField;
		}
		set
		{
			prstField = value;
		}
	}

	public static CT_Bevel Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Bevel cT_Bevel = new CT_Bevel();
		cT_Bevel.w = XmlHelper.ReadLong(node.Attributes["w"]);
		cT_Bevel.h = XmlHelper.ReadLong(node.Attributes["h"]);
		if (node.Attributes["prst"] != null)
		{
			cT_Bevel.prst = (ST_BevelPresetType)Enum.Parse(typeof(ST_BevelPresetType), node.Attributes["prst"].Value);
		}
		return cT_Bevel;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w", w, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "h", h, writeIfBlank: true);
		if (prst != ST_BevelPresetType.circle)
		{
			XmlHelper.WriteAttribute(sw, "prst", prst.ToString());
		}
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}

	public CT_Bevel()
	{
		wField = 76200L;
		hField = 76200L;
		prstField = ST_BevelPresetType.circle;
	}
}
