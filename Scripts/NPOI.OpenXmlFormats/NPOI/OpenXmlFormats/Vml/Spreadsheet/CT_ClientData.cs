using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;
using NPOI.OpenXmlFormats.Util;

namespace NPOI.OpenXmlFormats.Vml.Spreadsheet;

[DesignerCategory("code")]
public class CT_ClientData
{
	private ST_ObjectType objectTypeField;

	private static XmlQualifiedName MOVEWITHCELLS = new XmlQualifiedName("MoveWithCells", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SIZEWITHCELLS = new XmlQualifiedName("SizeWithCells", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName ANCHOR = new XmlQualifiedName("Anchor", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName LOCKED = new XmlQualifiedName("Locked", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DEFAULTSIZE = new XmlQualifiedName("DefaultSize", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName PRINTOBJECT = new XmlQualifiedName("PrintObject", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DISABLED = new XmlQualifiedName("Disabled", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName AUTOFILL = new XmlQualifiedName("AutoFill", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName AUTOLINE = new XmlQualifiedName("AutoLine", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName AUTOPICT = new XmlQualifiedName("AutoPict", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FMLAMACRO = new XmlQualifiedName("FmlaMacro", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName TEXTHALIGN = new XmlQualifiedName("TextHAlign", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName TEXTVALIGN = new XmlQualifiedName("TextVAlign", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName LOCKTEXT = new XmlQualifiedName("LockText", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName JUSTLASTX = new XmlQualifiedName("JustLastX", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SECRETEDIT = new XmlQualifiedName("SecretEdit", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DEFAULT = new XmlQualifiedName("Default", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName HELP = new XmlQualifiedName("Help", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName CANCEL = new XmlQualifiedName("Cancel", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DISMISS = new XmlQualifiedName("Dismiss", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName ACCEL = new XmlQualifiedName("Accel", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName ACCEL2 = new XmlQualifiedName("Accel2", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName ROW = new XmlQualifiedName("Row", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName COLUMN = new XmlQualifiedName("Column", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName VISIBLE = new XmlQualifiedName("Visible", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName ROWHIDDEN = new XmlQualifiedName("RowHidden", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName COLHIDDEN = new XmlQualifiedName("ColHidden", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName VTEDIT = new XmlQualifiedName("VTEdit", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName MULTILINE = new XmlQualifiedName("MultiLine", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName VSCROLL = new XmlQualifiedName("VScroll", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName VALIDIDS = new XmlQualifiedName("ValidIds", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FMLARANGE = new XmlQualifiedName("FmlaRange", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName WIDTHMIN = new XmlQualifiedName("WidthMin", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SEL = new XmlQualifiedName("Sel", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName NOTHREED2 = new XmlQualifiedName("NoThreeD2", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SELTYPE = new XmlQualifiedName("SelType", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName MULTISEL = new XmlQualifiedName("MultiSel", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName LCT = new XmlQualifiedName("LCT", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName LISTITEM = new XmlQualifiedName("ListItem", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DROPSTYLE = new XmlQualifiedName("DropStyle", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName COLORED = new XmlQualifiedName("Colored", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DROPLINES = new XmlQualifiedName("DropLines", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName CHECKED = new XmlQualifiedName("Checked", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FMLALINK = new XmlQualifiedName("FmlaLink", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FMLAPICT = new XmlQualifiedName("FmlaPict", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName NOTHREED = new XmlQualifiedName("NoThreeD", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FIRSTBUTTON = new XmlQualifiedName("FirstButton", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FMLAGROUP = new XmlQualifiedName("FmlaGroup", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName VAL = new XmlQualifiedName("Val", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName MIN = new XmlQualifiedName("Min", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName MAX = new XmlQualifiedName("Max", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName INC = new XmlQualifiedName("Inc", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName PAGE = new XmlQualifiedName("Page", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName HORIZ = new XmlQualifiedName("Horiz", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DX = new XmlQualifiedName("Dx", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName MAPOCX = new XmlQualifiedName("MapOCX", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName CF = new XmlQualifiedName("CF", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName CAMERA = new XmlQualifiedName("Camera", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName RECALCALWAYS = new XmlQualifiedName("RecalcAlways", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName AUTOSCALE = new XmlQualifiedName("AutoScale", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName DDE = new XmlQualifiedName("DDE", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName UIOBJ = new XmlQualifiedName("UIObj", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SCRIPTTEXT = new XmlQualifiedName("ScriptText", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SCRIPTEXTENDED = new XmlQualifiedName("ScriptExtended", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SCRIPTLANGUAGE = new XmlQualifiedName("ScriptLanguage", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName SCRIPTLOCATION = new XmlQualifiedName("ScriptLocation", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName FMLATXBX = new XmlQualifiedName("FmlaTxbx", "urn:schemas-microsoft-com:office:excel");

	private static XmlQualifiedName OBJECTTYPE = new XmlQualifiedName("ObjectType", "");

	private string anchorField;

	private ST_TrueFalseBlank autoFillField;

	private bool autoFillFieldSpecified;

	private ST_TrueFalseBlank visibleField;

	private bool visibleFieldSpecified;

	private ST_TrueFalseBlank moveWithCellsField;

	private bool moveWithCellsFieldSpecified;

	private ST_TrueFalseBlank sizeWithCellsField;

	private bool sizeWithCellsFieldSpecified;

	private List<int> columnField;

	private List<int> rowField;

	[XmlElement(ElementName = "Anchor")]
	public string anchor
	{
		get
		{
			return anchorField;
		}
		set
		{
			anchorField = value;
		}
	}

	[XmlElement(ElementName = "AutoFill")]
	[DefaultValue(ST_TrueFalseBlank.NONE)]
	public ST_TrueFalseBlank autoFill
	{
		get
		{
			return autoFillField;
		}
		set
		{
			autoFillField = value;
		}
	}

	[XmlIgnore]
	public bool autoFillSpecified
	{
		get
		{
			return autoFillFieldSpecified;
		}
		set
		{
			autoFillFieldSpecified = value;
		}
	}

	[XmlElement(ElementName = "Visible")]
	[DefaultValue(ST_TrueFalseBlank.NONE)]
	public ST_TrueFalseBlank visible
	{
		get
		{
			return visibleField;
		}
		set
		{
			visibleField = value;
		}
	}

	[XmlIgnore]
	public bool visibleSpecified
	{
		get
		{
			return visibleFieldSpecified;
		}
		set
		{
			visibleFieldSpecified = value;
		}
	}

	[XmlElement(ElementName = "MoveWithCells")]
	[DefaultValue(ST_TrueFalseBlank.NONE)]
	public ST_TrueFalseBlank moveWithCells
	{
		get
		{
			return moveWithCellsField;
		}
		set
		{
			moveWithCellsField = value;
		}
	}

	[XmlIgnore]
	public bool moveWithCellsSpecified
	{
		get
		{
			return moveWithCellsFieldSpecified;
		}
		set
		{
			moveWithCellsFieldSpecified = value;
		}
	}

	[XmlElement(ElementName = "SizeWithCells")]
	[DefaultValue(ST_TrueFalseBlank.NONE)]
	public ST_TrueFalseBlank sizeWithCells
	{
		get
		{
			return sizeWithCellsField;
		}
		set
		{
			sizeWithCellsField = value;
		}
	}

	[XmlIgnore]
	public bool sizeWithCellsSpecified
	{
		get
		{
			return sizeWithCellsFieldSpecified;
		}
		set
		{
			sizeWithCellsFieldSpecified = value;
		}
	}

	[XmlElement(ElementName = "Column")]
	public List<int> column
	{
		get
		{
			return columnField;
		}
		set
		{
			columnField = value;
		}
	}

	[XmlElement(ElementName = "Row")]
	public List<int> row
	{
		get
		{
			return rowField;
		}
		set
		{
			rowField = value;
		}
	}

	[XmlAttribute]
	public ST_ObjectType ObjectType
	{
		get
		{
			return objectTypeField;
		}
		set
		{
			objectTypeField = value;
		}
	}

	public CT_ClientData()
	{
		rowField = new List<int>();
		columnField = new List<int>();
	}

	public static CT_ClientData Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ClientData cT_ClientData = new CT_ClientData();
		if (node.Attributes["ObjectType"] != null)
		{
			cT_ClientData.ObjectType = (ST_ObjectType)Enum.Parse(typeof(ST_ObjectType), node.Attributes["ObjectType"].Value);
		}
		cT_ClientData.column = new List<int>();
		cT_ClientData.row = new List<int>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "Anchor")
			{
				cT_ClientData.anchor = childNode.InnerText;
			}
			else if (childNode.LocalName == "AutoFill")
			{
				cT_ClientData.autoFill = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalseBlank(childNode.InnerText);
			}
			else if (childNode.LocalName == "Visible")
			{
				cT_ClientData.visible = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalseBlank(childNode.InnerText);
			}
			else if (childNode.LocalName == "MoveWithCells")
			{
				cT_ClientData.moveWithCells = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalseBlank(childNode.InnerText);
			}
			else if (childNode.LocalName == "SizeWithCells")
			{
				cT_ClientData.sizeWithCells = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalseBlank(childNode.InnerText);
			}
			else if (childNode.LocalName == "Column")
			{
				cT_ClientData.column.Add(int.Parse(childNode.InnerText));
			}
			else if (childNode.LocalName == "Row")
			{
				cT_ClientData.row.Add(int.Parse(childNode.InnerText));
			}
		}
		return cT_ClientData;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<x:{nodeName}");
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "ObjectType", ObjectType.ToString());
		sw.Write(">");
		if (moveWithCells == ST_TrueFalseBlank.t || moveWithCells == ST_TrueFalseBlank.@true)
		{
			sw.Write(string.Format("<x:MoveWithCells/>", moveWithCells));
		}
		if (sizeWithCells == ST_TrueFalseBlank.t || sizeWithCells == ST_TrueFalseBlank.@true)
		{
			sw.Write(string.Format("<x:SizeWithCells/>", sizeWithCells));
		}
		if (anchor != null)
		{
			sw.Write($"<x:Anchor>{anchor}</x:Anchor>");
		}
		if (autoFill != ST_TrueFalseBlank.NONE)
		{
			sw.Write($"<x:AutoFill>{autoFill}</x:AutoFill>");
		}
		if (visible != ST_TrueFalseBlank.NONE)
		{
			sw.Write($"<x:Visible>{visible}</x:Visible>");
		}
		if (row != null)
		{
			foreach (int item in row)
			{
				sw.Write($"<x:Row>{item}</x:Row>");
			}
		}
		if (column != null)
		{
			foreach (int item2 in column)
			{
				sw.Write($"<x:Column>{item2}</x:Column>");
			}
		}
		sw.Write($"</x:{nodeName}>");
	}

	public void AddNewRow(int rowNum)
	{
		if (rowField != null)
		{
			rowField.Add(rowNum);
		}
	}

	public void AddNewColumn(int columnNum)
	{
		if (columnField != null)
		{
			columnField.Add(columnNum);
		}
	}

	public void AddNewMoveWithCells()
	{
		moveWithCellsField = ST_TrueFalseBlank.t;
		moveWithCellsFieldSpecified = true;
	}

	public void AddNewSizeWithCells()
	{
		sizeWithCellsField = ST_TrueFalseBlank.t;
		sizeWithCellsFieldSpecified = true;
	}

	public void AddNewAnchor(string name)
	{
		anchorField = name;
	}

	public void AddNewAutoFill(ST_TrueFalseBlank value)
	{
		autoFillField = value;
		autoFillFieldSpecified = true;
	}

	public int SizeOfMoveWithCellsArray()
	{
		if (!moveWithCellsSpecified)
		{
			return 0;
		}
		return 1;
	}

	public int SizeOfSizeWithCellsArray()
	{
		if (!sizeWithCellsFieldSpecified)
		{
			return 0;
		}
		return 1;
	}

	public int GetColumnArray(int index)
	{
		return columnField[index];
	}

	public void SetColumnArray(int index, int value)
	{
		columnField[index] = value;
	}

	public void SetRowArray(int index, int value)
	{
		rowField[index] = value;
	}

	public void SetAnchorArray(int index, string value)
	{
		AddNewAnchor(value);
	}

	public int GetRowArray(int index)
	{
		return rowField[index];
	}

	public string GetAnchorArray(int p)
	{
		return anchor;
	}
}
