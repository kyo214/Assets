using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_FileRecoveryPr
{
	private bool autoRecoverField;

	private bool crashSaveField;

	private bool dataExtractLoadField;

	private bool repairLoadField;

	[DefaultValue(true)]
	public bool autoRecover
	{
		get
		{
			return autoRecoverField;
		}
		set
		{
			autoRecoverField = value;
		}
	}

	[DefaultValue(false)]
	public bool crashSave
	{
		get
		{
			return crashSaveField;
		}
		set
		{
			crashSaveField = value;
		}
	}

	[DefaultValue(false)]
	public bool dataExtractLoad
	{
		get
		{
			return dataExtractLoadField;
		}
		set
		{
			dataExtractLoadField = value;
		}
	}

	[DefaultValue(false)]
	public bool repairLoad
	{
		get
		{
			return repairLoadField;
		}
		set
		{
			repairLoadField = value;
		}
	}

	public static CT_FileRecoveryPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_FileRecoveryPr
		{
			autoRecover = XmlHelper.ReadBool(node.Attributes["autoRecover"]),
			crashSave = XmlHelper.ReadBool(node.Attributes["crashSave"]),
			dataExtractLoad = XmlHelper.ReadBool(node.Attributes["dataExtractLoad"]),
			repairLoad = XmlHelper.ReadBool(node.Attributes["repairLoad"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "autoRecover", autoRecover);
		XmlHelper.WriteAttribute(sw, "crashSave", crashSave);
		XmlHelper.WriteAttribute(sw, "dataExtractLoad", dataExtractLoad);
		XmlHelper.WriteAttribute(sw, "repairLoad", repairLoad);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}

	public CT_FileRecoveryPr()
	{
		autoRecoverField = true;
		crashSaveField = false;
		dataExtractLoadField = false;
		repairLoadField = false;
	}
}
