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
public class CT_GraphicalObjectFrameLocking
{
	private CT_OfficeArtExtensionList extLstField;

	private bool noGrpField;

	private bool noDrilldownField;

	private bool noSelectField;

	private bool noChangeAspectField;

	private bool noMoveField;

	private bool noResizeField;

	[XmlElement(Order = 0)]
	public CT_OfficeArtExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool noGrp
	{
		get
		{
			return noGrpField;
		}
		set
		{
			noGrpField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool noDrilldown
	{
		get
		{
			return noDrilldownField;
		}
		set
		{
			noDrilldownField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool noSelect
	{
		get
		{
			return noSelectField;
		}
		set
		{
			noSelectField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool noChangeAspect
	{
		get
		{
			return noChangeAspectField;
		}
		set
		{
			noChangeAspectField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool noMove
	{
		get
		{
			return noMoveField;
		}
		set
		{
			noMoveField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool noResize
	{
		get
		{
			return noResizeField;
		}
		set
		{
			noResizeField = value;
		}
	}

	public CT_GraphicalObjectFrameLocking()
	{
		noGrpField = false;
		noDrilldownField = false;
		noSelectField = false;
		noChangeAspectField = false;
		noMoveField = false;
		noResizeField = false;
	}

	public static CT_GraphicalObjectFrameLocking Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GraphicalObjectFrameLocking cT_GraphicalObjectFrameLocking = new CT_GraphicalObjectFrameLocking();
		cT_GraphicalObjectFrameLocking.noGrp = XmlHelper.ReadBool(node.Attributes["noGrp"]);
		cT_GraphicalObjectFrameLocking.noDrilldown = XmlHelper.ReadBool(node.Attributes["noDrilldown"]);
		cT_GraphicalObjectFrameLocking.noSelect = XmlHelper.ReadBool(node.Attributes["noSelect"]);
		cT_GraphicalObjectFrameLocking.noChangeAspect = XmlHelper.ReadBool(node.Attributes["noChangeAspect"]);
		cT_GraphicalObjectFrameLocking.noMove = XmlHelper.ReadBool(node.Attributes["noMove"]);
		cT_GraphicalObjectFrameLocking.noResize = XmlHelper.ReadBool(node.Attributes["noResize"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_GraphicalObjectFrameLocking.extLst = CT_OfficeArtExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_GraphicalObjectFrameLocking;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName} xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"");
		XmlHelper.WriteAttribute(sw, "noGrp", noGrp, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "noDrilldown", noDrilldown, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "noSelect", noSelect, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "noChangeAspect", noChangeAspect, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "noMove", noMove, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "noResize", noResize, writeIfBlank: false);
		if (extLst != null && extLst.ext.Count != 0)
		{
			sw.Write(">");
			extLst.Write(sw, "extLst");
			sw.Write($"</a:{nodeName}>");
		}
		else
		{
			sw.Write("/>");
		}
	}
}
