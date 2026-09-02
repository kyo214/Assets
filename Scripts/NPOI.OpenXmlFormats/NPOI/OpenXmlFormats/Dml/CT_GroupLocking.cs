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
public class CT_GroupLocking
{
	private CT_OfficeArtExtensionList extLstField;

	private bool noGrpField;

	private bool noUngrpField;

	private bool noSelectField;

	private bool noRotField;

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
	public bool noUngrp
	{
		get
		{
			return noUngrpField;
		}
		set
		{
			noUngrpField = value;
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
	public bool noRot
	{
		get
		{
			return noRotField;
		}
		set
		{
			noRotField = value;
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

	public CT_GroupLocking()
	{
		noGrpField = false;
		noUngrpField = false;
		noSelectField = false;
		noRotField = false;
		noChangeAspectField = false;
		noMoveField = false;
		noResizeField = false;
	}

	public static CT_GroupLocking Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GroupLocking cT_GroupLocking = new CT_GroupLocking();
		cT_GroupLocking.noGrp = XmlHelper.ReadBool(node.Attributes["noGrp"]);
		cT_GroupLocking.noUngrp = XmlHelper.ReadBool(node.Attributes["noUngrp"]);
		cT_GroupLocking.noSelect = XmlHelper.ReadBool(node.Attributes["noSelect"]);
		cT_GroupLocking.noRot = XmlHelper.ReadBool(node.Attributes["noRot"]);
		cT_GroupLocking.noChangeAspect = XmlHelper.ReadBool(node.Attributes["noChangeAspect"]);
		cT_GroupLocking.noMove = XmlHelper.ReadBool(node.Attributes["noMove"]);
		cT_GroupLocking.noResize = XmlHelper.ReadBool(node.Attributes["noResize"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_GroupLocking.extLst = CT_OfficeArtExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_GroupLocking;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		if (noGrp)
		{
			XmlHelper.WriteAttribute(sw, "noGrp", noGrp);
		}
		if (noUngrp)
		{
			XmlHelper.WriteAttribute(sw, "noUngrp", noUngrp);
		}
		if (noSelect)
		{
			XmlHelper.WriteAttribute(sw, "noSelect", noSelect);
		}
		if (noRot)
		{
			XmlHelper.WriteAttribute(sw, "noRot", noRot);
		}
		if (noChangeAspect)
		{
			XmlHelper.WriteAttribute(sw, "noChangeAspect", noChangeAspect);
		}
		if (noMove)
		{
			XmlHelper.WriteAttribute(sw, "noMove", noMove);
		}
		if (noResize)
		{
			XmlHelper.WriteAttribute(sw, "noResize", noResize);
		}
		sw.Write("/>");
	}
}
