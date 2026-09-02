using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PageField
{
	private CT_ExtensionList extLstField;

	private int fldField;

	private uint itemField;

	private bool itemFieldSpecified;

	private int hierField;

	private bool hierFieldSpecified;

	private string nameField;

	private string capField;

	[XmlElement(Order = 0)]
	public CT_ExtensionList extLst
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
	public int fld
	{
		get
		{
			return fldField;
		}
		set
		{
			fldField = value;
		}
	}

	[XmlAttribute]
	public uint item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}

	[XmlIgnore]
	public bool itemSpecified
	{
		get
		{
			return itemFieldSpecified;
		}
		set
		{
			itemFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int hier
	{
		get
		{
			return hierField;
		}
		set
		{
			hierField = value;
		}
	}

	[XmlIgnore]
	public bool hierSpecified
	{
		get
		{
			return hierFieldSpecified;
		}
		set
		{
			hierFieldSpecified = value;
		}
	}

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
	public string cap
	{
		get
		{
			return capField;
		}
		set
		{
			capField = value;
		}
	}

	public static CT_PageField Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PageField cT_PageField = new CT_PageField();
		if (node.Attributes["fld"] != null)
		{
			cT_PageField.fld = XmlHelper.ReadInt(node.Attributes["fld"]);
		}
		if (node.Attributes["item"] != null)
		{
			cT_PageField.item = XmlHelper.ReadUInt(node.Attributes["item"]);
		}
		if (node.Attributes["hier"] != null)
		{
			cT_PageField.hier = XmlHelper.ReadInt(node.Attributes["hier"]);
		}
		cT_PageField.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_PageField.cap = XmlHelper.ReadString(node.Attributes["cap"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_PageField.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_PageField;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "fld", fld);
		XmlHelper.WriteAttribute(sw, "item", item);
		XmlHelper.WriteAttribute(sw, "hier", hier);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "cap", cap);
		sw.Write(">");
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PageField()
	{
		extLstField = new CT_ExtensionList();
	}
}
