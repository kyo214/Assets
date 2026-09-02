using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_RangeSet
{
	private uint i1Field;

	private bool i1FieldSpecified;

	private uint i2Field;

	private bool i2FieldSpecified;

	private uint i3Field;

	private bool i3FieldSpecified;

	private uint i4Field;

	private bool i4FieldSpecified;

	private string refField;

	private string nameField;

	private string sheetField;

	private string idField;

	[XmlAttribute]
	public uint i1
	{
		get
		{
			return i1Field;
		}
		set
		{
			i1Field = value;
		}
	}

	[XmlIgnore]
	public bool i1Specified
	{
		get
		{
			return i1FieldSpecified;
		}
		set
		{
			i1FieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint i2
	{
		get
		{
			return i2Field;
		}
		set
		{
			i2Field = value;
		}
	}

	[XmlIgnore]
	public bool i2Specified
	{
		get
		{
			return i2FieldSpecified;
		}
		set
		{
			i2FieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint i3
	{
		get
		{
			return i3Field;
		}
		set
		{
			i3Field = value;
		}
	}

	[XmlIgnore]
	public bool i3Specified
	{
		get
		{
			return i3FieldSpecified;
		}
		set
		{
			i3FieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint i4
	{
		get
		{
			return i4Field;
		}
		set
		{
			i4Field = value;
		}
	}

	[XmlIgnore]
	public bool i4Specified
	{
		get
		{
			return i4FieldSpecified;
		}
		set
		{
			i4FieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string @ref
	{
		get
		{
			return refField;
		}
		set
		{
			refField = value;
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
	public string sheet
	{
		get
		{
			return sheetField;
		}
		set
		{
			sheetField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships")]
	public string id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	public static CT_RangeSet Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RangeSet cT_RangeSet = new CT_RangeSet();
		if (node.Attributes["i1"] != null)
		{
			cT_RangeSet.i1 = XmlHelper.ReadUInt(node.Attributes["i1"]);
		}
		if (node.Attributes["i2"] != null)
		{
			cT_RangeSet.i2 = XmlHelper.ReadUInt(node.Attributes["i2"]);
		}
		if (node.Attributes["i3"] != null)
		{
			cT_RangeSet.i3 = XmlHelper.ReadUInt(node.Attributes["i3"]);
		}
		if (node.Attributes["i4"] != null)
		{
			cT_RangeSet.i4 = XmlHelper.ReadUInt(node.Attributes["i4"]);
		}
		cT_RangeSet.@ref = XmlHelper.ReadString(node.Attributes["ref"]);
		cT_RangeSet.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_RangeSet.sheet = XmlHelper.ReadString(node.Attributes["sheet"]);
		cT_RangeSet.id = XmlHelper.ReadString(node.Attributes["r:id"]);
		return cT_RangeSet;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "i1", i1);
		XmlHelper.WriteAttribute(sw, "i2", i2);
		XmlHelper.WriteAttribute(sw, "i3", i3);
		XmlHelper.WriteAttribute(sw, "i4", i4);
		XmlHelper.WriteAttribute(sw, "ref", @ref);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "sheet", sheet);
		XmlHelper.WriteAttribute(sw, "r:id", id);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
