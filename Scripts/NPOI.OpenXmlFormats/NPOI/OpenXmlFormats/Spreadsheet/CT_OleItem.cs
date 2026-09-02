using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_OleItem
{
	private string nameField;

	private bool iconField;

	private bool adviseField;

	private bool preferPicField;

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
	[DefaultValue(false)]
	public bool icon
	{
		get
		{
			return iconField;
		}
		set
		{
			iconField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool advise
	{
		get
		{
			return adviseField;
		}
		set
		{
			adviseField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool preferPic
	{
		get
		{
			return preferPicField;
		}
		set
		{
			preferPicField = value;
		}
	}

	public CT_OleItem()
	{
		iconField = false;
		adviseField = false;
		preferPicField = false;
	}

	internal static CT_OleItem Parse(XmlNode node)
	{
		return new CT_OleItem
		{
			name = XmlHelper.ReadString(node.Attributes["name"]),
			advise = XmlHelper.ReadBool(node.Attributes["advise"]),
			icon = XmlHelper.ReadBool(node.Attributes["icon"]),
			preferPic = XmlHelper.ReadBool(node.Attributes["preferPic"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "advise", advise);
		XmlHelper.WriteAttribute(sw, "icon", iconField, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "preferPic", preferPic);
		sw.Write(string.Format("/>", nodeName));
	}
}
