using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_WorkbookProtection
{
	private byte[] workbookPasswordField;

	private byte[] revisionsPasswordField;

	private bool lockStructureField;

	private bool lockWindowsField;

	private bool lockRevisionField;

	[XmlAttribute]
	public byte[] workbookPassword
	{
		get
		{
			return workbookPasswordField;
		}
		set
		{
			workbookPasswordField = value;
		}
	}

	[XmlAttribute]
	public byte[] revisionsPassword
	{
		get
		{
			return revisionsPasswordField;
		}
		set
		{
			revisionsPasswordField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool lockStructure
	{
		get
		{
			return lockStructureField;
		}
		set
		{
			lockStructureField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool lockWindows
	{
		get
		{
			return lockWindowsField;
		}
		set
		{
			lockWindowsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool lockRevision
	{
		get
		{
			return lockRevisionField;
		}
		set
		{
			lockRevisionField = value;
		}
	}

	public static CT_WorkbookProtection Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_WorkbookProtection
		{
			workbookPassword = XmlHelper.ReadBytes(node.Attributes["workbookPassword"]),
			revisionsPassword = XmlHelper.ReadBytes(node.Attributes["revisionsPassword"]),
			lockStructure = XmlHelper.ReadBool(node.Attributes["lockStructure"]),
			lockWindows = XmlHelper.ReadBool(node.Attributes["lockWindows"]),
			lockRevision = XmlHelper.ReadBool(node.Attributes["lockRevision"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "workbookPassword", workbookPassword);
		XmlHelper.WriteAttribute(sw, "revisionsPassword", revisionsPassword);
		XmlHelper.WriteAttribute(sw, "lockStructure", lockStructure);
		XmlHelper.WriteAttribute(sw, "lockWindows", lockWindows);
		XmlHelper.WriteAttribute(sw, "lockRevision", lockRevision);
		sw.Write("/>");
	}

	public CT_WorkbookProtection()
	{
		lockStructureField = false;
		lockWindowsField = false;
		lockRevisionField = false;
	}
}
