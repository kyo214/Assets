using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_StyleMatrix
{
	private CT_FillStyleList fillStyleLstField;

	private CT_LineStyleList lnStyleLstField;

	private CT_EffectStyleList effectStyleLstField;

	private CT_BackgroundFillStyleList bgFillStyleLstField;

	private string nameField;

	[XmlElement(Order = 0)]
	public CT_FillStyleList fillStyleLst
	{
		get
		{
			return fillStyleLstField;
		}
		set
		{
			fillStyleLstField = value;
		}
	}

	[XmlArray(Order = 1)]
	[XmlArrayItem("ln", IsNullable = false)]
	public CT_LineStyleList lnStyleLst
	{
		get
		{
			return lnStyleLstField;
		}
		set
		{
			lnStyleLstField = value;
		}
	}

	[XmlArray(Order = 2)]
	[XmlArrayItem("effectStyle", IsNullable = false)]
	public CT_EffectStyleList effectStyleLst
	{
		get
		{
			return effectStyleLstField;
		}
		set
		{
			effectStyleLstField = value;
		}
	}

	[XmlElement(Order = 3)]
	public CT_BackgroundFillStyleList bgFillStyleLst
	{
		get
		{
			return bgFillStyleLstField;
		}
		set
		{
			bgFillStyleLstField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("")]
	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	public static CT_StyleMatrix Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_StyleMatrix cT_StyleMatrix = new CT_StyleMatrix();
		cT_StyleMatrix.name = XmlHelper.ReadString(node.Attributes["name"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "fillStyleLst")
			{
				cT_StyleMatrix.fillStyleLst = CT_FillStyleList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "bgFillStyleLst")
			{
				cT_StyleMatrix.bgFillStyleLst = CT_BackgroundFillStyleList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "lnStyleLst")
			{
				cT_StyleMatrix.lnStyleLst = CT_LineStyleList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "effectStyleLst")
			{
				cT_StyleMatrix.effectStyleLst = CT_EffectStyleList.Parse(childNode, namespaceManager);
			}
		}
		return cT_StyleMatrix;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		sw.Write(">");
		if (fillStyleLst != null)
		{
			fillStyleLst.Write(sw, "fillStyleLst");
		}
		if (lnStyleLst != null)
		{
			lnStyleLst.Write(sw, "lnStyleLst");
		}
		if (effectStyleLst != null)
		{
			effectStyleLst.Write(sw, "effectStyleLst");
		}
		if (bgFillStyleLst != null)
		{
			bgFillStyleLst.Write(sw, "bgFillStyleLst");
		}
		sw.Write($"</a:{nodeName}>");
	}

	public CT_StyleMatrix()
	{
		nameField = "";
	}
}
