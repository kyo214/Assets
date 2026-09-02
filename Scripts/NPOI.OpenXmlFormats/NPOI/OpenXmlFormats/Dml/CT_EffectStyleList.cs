using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_EffectStyleList
{
	private List<CT_EffectStyleItem> effectStyleField;

	[XmlElement("effectStyle", Order = 0)]
	public List<CT_EffectStyleItem> effectStyle
	{
		get
		{
			return effectStyleField;
		}
		set
		{
			effectStyleField = value;
		}
	}

	public CT_EffectStyleList()
	{
		effectStyleField = new List<CT_EffectStyleItem>();
	}

	public static CT_EffectStyleList Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_EffectStyleList cT_EffectStyleList = new CT_EffectStyleList();
		cT_EffectStyleList.effectStyle = new List<CT_EffectStyleItem>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			cT_EffectStyleList.effectStyle.Add(CT_EffectStyleItem.Parse(childNode, namespaceManager));
		}
		return cT_EffectStyleList;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}>");
		if (effectStyle.Count > 0)
		{
			foreach (CT_EffectStyleItem item in effectStyle)
			{
				item.Write(sw, "effectStyle");
			}
		}
		sw.Write($"</a:{nodeName}>");
	}
}
