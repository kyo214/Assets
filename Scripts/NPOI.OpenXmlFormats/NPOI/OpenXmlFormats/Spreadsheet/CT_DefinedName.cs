using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_DefinedName
{
	private string nameField;

	private string commentField;

	private string customMenuField;

	private string descriptionField;

	private string helpField;

	private string statusBarField;

	private uint localSheetIdField;

	private bool localSheetIdFieldSpecified;

	private bool hiddenField;

	private bool functionField;

	private bool vbProcedureField;

	private bool xlmField;

	private uint functionGroupIdField;

	private bool functionGroupIdFieldSpecified;

	private string shortcutKeyField;

	private bool publishToServerField;

	private bool workbookParameterField;

	private string valueField;

	[XmlAttribute]
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

	[XmlAttribute]
	public string comment
	{
		get
		{
			return commentField;
		}
		set
		{
			commentField = value;
		}
	}

	[XmlAttribute]
	public string customMenu
	{
		get
		{
			return customMenuField;
		}
		set
		{
			customMenuField = value;
		}
	}

	[XmlAttribute]
	public string description
	{
		get
		{
			return descriptionField;
		}
		set
		{
			descriptionField = value;
		}
	}

	[XmlAttribute]
	public string help
	{
		get
		{
			return helpField;
		}
		set
		{
			helpField = value;
		}
	}

	[XmlAttribute]
	public string statusBar
	{
		get
		{
			return statusBarField;
		}
		set
		{
			statusBarField = value;
		}
	}

	[XmlAttribute]
	public uint localSheetId
	{
		get
		{
			return localSheetIdField;
		}
		set
		{
			localSheetIdField = value;
			localSheetIdFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool localSheetIdSpecified
	{
		get
		{
			return localSheetIdFieldSpecified;
		}
		set
		{
			localSheetIdFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool hidden
	{
		get
		{
			return hiddenField;
		}
		set
		{
			hiddenField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool function
	{
		get
		{
			return functionField;
		}
		set
		{
			functionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool vbProcedure
	{
		get
		{
			return vbProcedureField;
		}
		set
		{
			vbProcedureField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool xlm
	{
		get
		{
			return xlmField;
		}
		set
		{
			xlmField = value;
		}
	}

	[XmlAttribute]
	public uint functionGroupId
	{
		get
		{
			return functionGroupIdField;
		}
		set
		{
			functionGroupIdField = value;
		}
	}

	[XmlIgnore]
	public bool functionGroupIdSpecified
	{
		get
		{
			return functionGroupIdFieldSpecified;
		}
		set
		{
			functionGroupIdFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string shortcutKey
	{
		get
		{
			return shortcutKeyField;
		}
		set
		{
			shortcutKeyField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool publishToServer
	{
		get
		{
			return publishToServerField;
		}
		set
		{
			publishToServerField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool workbookParameter
	{
		get
		{
			return workbookParameterField;
		}
		set
		{
			workbookParameterField = value;
		}
	}

	[XmlText]
	public string Value
	{
		get
		{
			return valueField;
		}
		set
		{
			valueField = value;
		}
	}

	public static CT_DefinedName Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_DefinedName
		{
			name = XmlHelper.ReadString(node.Attributes["name"]),
			comment = XmlHelper.ReadString(node.Attributes["comment"]),
			customMenu = XmlHelper.ReadString(node.Attributes["customMenu"]),
			description = XmlHelper.ReadString(node.Attributes["description"]),
			help = XmlHelper.ReadString(node.Attributes["help"]),
			statusBar = XmlHelper.ReadString(node.Attributes["statusBar"]),
			localSheetId = XmlHelper.ReadUInt(node.Attributes["localSheetId"]),
			localSheetIdFieldSpecified = (node.Attributes["localSheetId"] != null),
			hidden = XmlHelper.ReadBool(node.Attributes["hidden"]),
			function = XmlHelper.ReadBool(node.Attributes["function"]),
			vbProcedure = XmlHelper.ReadBool(node.Attributes["vbProcedure"]),
			xlm = XmlHelper.ReadBool(node.Attributes["xlm"]),
			functionGroupId = XmlHelper.ReadUInt(node.Attributes["functionGroupId"]),
			shortcutKey = XmlHelper.ReadString(node.Attributes["shortcutKey"]),
			publishToServer = XmlHelper.ReadBool(node.Attributes["publishToServer"]),
			workbookParameter = XmlHelper.ReadBool(node.Attributes["workbookParameter"]),
			Value = node.InnerText
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "comment", comment);
		XmlHelper.WriteAttribute(sw, "customMenu", customMenu);
		XmlHelper.WriteAttribute(sw, "description", description);
		XmlHelper.WriteAttribute(sw, "help", help);
		XmlHelper.WriteAttribute(sw, "statusBar", statusBar);
		if (localSheetIdFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "localSheetId", localSheetId, writeIfBlank: true);
		}
		if (hidden)
		{
			XmlHelper.WriteAttribute(sw, "hidden", hidden);
		}
		if (function)
		{
			XmlHelper.WriteAttribute(sw, "function", function);
		}
		if (vbProcedure)
		{
			XmlHelper.WriteAttribute(sw, "vbProcedure", vbProcedure);
		}
		if (xlm)
		{
			XmlHelper.WriteAttribute(sw, "xlm", xlm);
		}
		XmlHelper.WriteAttribute(sw, "functionGroupId", functionGroupId);
		XmlHelper.WriteAttribute(sw, "shortcutKey", shortcutKey);
		if (publishToServerField)
		{
			XmlHelper.WriteAttribute(sw, "publishToServer", publishToServer);
		}
		if (workbookParameterField)
		{
			XmlHelper.WriteAttribute(sw, "workbookParameter", workbookParameter);
		}
		sw.Write(">");
		sw.Write($"<![CDATA[{Value}]]>");
		sw.Write($"</{nodeName}>");
	}

	public CT_DefinedName()
	{
		hiddenField = false;
		functionField = false;
		vbProcedureField = false;
		xlmField = false;
		publishToServerField = false;
		workbookParameterField = false;
	}

	public bool IsSetLocalSheetId()
	{
		return localSheetIdFieldSpecified;
	}

	public void UnsetLocalSheetId()
	{
		localSheetIdFieldSpecified = false;
	}
}
