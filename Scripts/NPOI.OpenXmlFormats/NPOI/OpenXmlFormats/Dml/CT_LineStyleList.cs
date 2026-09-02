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
public class CT_LineStyleList
{
	private List<CT_LineProperties> lnField;

	[XmlElement("ln", Order = 0)]
	public List<CT_LineProperties> ln
	{
		get
		{
			return lnField;
		}
		set
		{
			lnField = value;
		}
	}

	public CT_LineStyleList()
	{
		lnField = new List<CT_LineProperties>();
	}

	public static CT_LineStyleList Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_LineStyleList cT_LineStyleList = new CT_LineStyleList();
		cT_LineStyleList.ln = new List<CT_LineProperties>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			cT_LineStyleList.ln.Add(CT_LineProperties.Parse(childNode, namespaceManager));
		}
		return cT_LineStyleList;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}>");
		if (ln.Count > 0)
		{
			foreach (CT_LineProperties item in ln)
			{
				item.Write(sw, "ln");
			}
		}
		sw.Write($"</a:{nodeName}>");
	}
}
