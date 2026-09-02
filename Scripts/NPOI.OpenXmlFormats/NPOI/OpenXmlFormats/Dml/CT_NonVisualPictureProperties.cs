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
public class CT_NonVisualPictureProperties
{
	private CT_PictureLocking picLocksField;

	private CT_OfficeArtExtensionList extLstField;

	private bool preferRelativeResizeField = true;

	[XmlElement(Order = 0)]
	public CT_PictureLocking picLocks
	{
		get
		{
			return picLocksField;
		}
		set
		{
			picLocksField = value;
		}
	}

	[XmlElement(Order = 1)]
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
	public bool preferRelativeResize
	{
		get
		{
			return preferRelativeResizeField;
		}
		set
		{
			preferRelativeResizeField = value;
		}
	}

	public static CT_NonVisualPictureProperties Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_NonVisualPictureProperties cT_NonVisualPictureProperties = new CT_NonVisualPictureProperties();
		cT_NonVisualPictureProperties.preferRelativeResize = XmlHelper.ReadBool(node.Attributes["preferRelativeResize"], blankValue: true);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "picLocks")
			{
				cT_NonVisualPictureProperties.picLocks = CT_PictureLocking.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_NonVisualPictureProperties.extLst = CT_OfficeArtExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_NonVisualPictureProperties;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<pic:{nodeName}");
		if (!preferRelativeResize)
		{
			XmlHelper.WriteAttribute(sw, "preferRelativeResize", preferRelativeResize);
		}
		sw.Write(">");
		if (picLocks != null)
		{
			picLocks.Write(sw, "picLocks");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</pic:{nodeName}>");
	}

	public CT_PictureLocking AddNewPicLocks()
	{
		picLocksField = new CT_PictureLocking();
		return picLocksField;
	}
}
