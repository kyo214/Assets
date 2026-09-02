using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_PageSetUpPr
{
	private bool autoPageBreaksField;

	private bool fitToPageField;

	[DefaultValue(true)]
	public bool autoPageBreaks
	{
		get
		{
			return autoPageBreaksField;
		}
		set
		{
			autoPageBreaksField = value;
		}
	}

	[DefaultValue(false)]
	public bool fitToPage
	{
		get
		{
			return fitToPageField;
		}
		set
		{
			fitToPageField = value;
		}
	}

	public static CT_PageSetUpPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_PageSetUpPr
		{
			autoPageBreaks = XmlHelper.ReadBool(node.Attributes["autoPageBreaks"]),
			fitToPage = XmlHelper.ReadBool(node.Attributes["fitToPage"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "autoPageBreaks", autoPageBreaks, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "fitToPage", fitToPage, writeIfBlank: false);
		sw.Write("/>");
	}

	public CT_PageSetUpPr()
	{
		autoPageBreaksField = true;
		fitToPageField = false;
	}

	public CT_PageSetUpPr Clone()
	{
		return new CT_PageSetUpPr
		{
			autoPageBreaksField = autoPageBreaksField,
			fitToPageField = fitToPageField
		};
	}
}
