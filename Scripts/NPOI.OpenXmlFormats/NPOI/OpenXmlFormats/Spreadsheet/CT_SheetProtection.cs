using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_SheetProtection
{
	private string passwordField;

	private bool sheetField;

	private bool objectsField;

	private bool scenariosField;

	private bool formatCellsField;

	private bool formatColumnsField;

	private bool formatRowsField;

	private bool insertColumnsField;

	private bool insertRowsField;

	private bool insertHyperlinksField;

	private bool deleteColumnsField;

	private bool deleteRowsField;

	private bool selectLockedCellsField;

	private bool sortField;

	private bool autoFilterField;

	private bool pivotTablesField;

	private bool selectUnlockedCellsField;

	public string algorithmName { get; set; }

	public string hashValue { get; set; }

	public string saltValue { get; set; }

	public string spinCount { get; set; }

	[XmlAttribute]
	public string password
	{
		get
		{
			return passwordField;
		}
		set
		{
			passwordField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool sheet
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

	[XmlAttribute]
	[DefaultValue(false)]
	public bool objects
	{
		get
		{
			return objectsField;
		}
		set
		{
			objectsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool scenarios
	{
		get
		{
			return scenariosField;
		}
		set
		{
			scenariosField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool formatCells
	{
		get
		{
			return formatCellsField;
		}
		set
		{
			formatCellsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool formatColumns
	{
		get
		{
			return formatColumnsField;
		}
		set
		{
			formatColumnsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool formatRows
	{
		get
		{
			return formatRowsField;
		}
		set
		{
			formatRowsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool insertColumns
	{
		get
		{
			return insertColumnsField;
		}
		set
		{
			insertColumnsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool insertRows
	{
		get
		{
			return insertRowsField;
		}
		set
		{
			insertRowsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool insertHyperlinks
	{
		get
		{
			return insertHyperlinksField;
		}
		set
		{
			insertHyperlinksField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool deleteColumns
	{
		get
		{
			return deleteColumnsField;
		}
		set
		{
			deleteColumnsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool deleteRows
	{
		get
		{
			return deleteRowsField;
		}
		set
		{
			deleteRowsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool selectLockedCells
	{
		get
		{
			return selectLockedCellsField;
		}
		set
		{
			selectLockedCellsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool sort
	{
		get
		{
			return sortField;
		}
		set
		{
			sortField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool autoFilter
	{
		get
		{
			return autoFilterField;
		}
		set
		{
			autoFilterField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool pivotTables
	{
		get
		{
			return pivotTablesField;
		}
		set
		{
			pivotTablesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool selectUnlockedCells
	{
		get
		{
			return selectUnlockedCellsField;
		}
		set
		{
			selectUnlockedCellsField = value;
		}
	}

	public CT_SheetProtection()
	{
		formatCellsField = true;
		formatColumnsField = true;
		formatRowsField = true;
		insertColumnsField = true;
		insertRowsField = true;
		insertHyperlinksField = true;
		deleteColumnsField = true;
		deleteRowsField = true;
		selectLockedCellsField = false;
		sortField = true;
		autoFilterField = true;
		pivotTablesField = true;
		selectUnlockedCellsField = false;
	}

	public static CT_SheetProtection Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_SheetProtection cT_SheetProtection = new CT_SheetProtection();
		cT_SheetProtection.password = XmlHelper.ReadString(node.Attributes["password"]);
		cT_SheetProtection.sheet = XmlHelper.ReadBool(node.Attributes["sheet"]);
		cT_SheetProtection.objects = XmlHelper.ReadBool(node.Attributes["objects"]);
		cT_SheetProtection.scenarios = XmlHelper.ReadBool(node.Attributes["scenarios"]);
		cT_SheetProtection.formatCells = XmlHelper.ReadBool(node.Attributes["formatCells"], blankValue: true);
		cT_SheetProtection.formatColumns = XmlHelper.ReadBool(node.Attributes["formatColumns"], blankValue: true);
		cT_SheetProtection.formatRows = XmlHelper.ReadBool(node.Attributes["formatRows"], blankValue: true);
		cT_SheetProtection.insertColumns = XmlHelper.ReadBool(node.Attributes["insertColumns"], blankValue: true);
		cT_SheetProtection.insertRows = XmlHelper.ReadBool(node.Attributes["insertRows"], blankValue: true);
		cT_SheetProtection.insertHyperlinks = XmlHelper.ReadBool(node.Attributes["insertHyperlinks"], blankValue: true);
		cT_SheetProtection.deleteColumns = XmlHelper.ReadBool(node.Attributes["deleteColumns"], blankValue: true);
		cT_SheetProtection.deleteRows = XmlHelper.ReadBool(node.Attributes["deleteRows"], blankValue: true);
		cT_SheetProtection.selectLockedCells = XmlHelper.ReadBool(node.Attributes["selectLockedCells"]);
		cT_SheetProtection.sort = XmlHelper.ReadBool(node.Attributes["sort"], blankValue: true);
		cT_SheetProtection.autoFilter = XmlHelper.ReadBool(node.Attributes["autoFilter"], blankValue: true);
		cT_SheetProtection.pivotTables = XmlHelper.ReadBool(node.Attributes["pivotTables"], blankValue: true);
		cT_SheetProtection.selectUnlockedCells = XmlHelper.ReadBool(node.Attributes["selectUnlockedCells"]);
		cT_SheetProtection.algorithmName = XmlHelper.ReadString(node.Attributes["algorithmName"]);
		if (cT_SheetProtection.algorithmName != null)
		{
			cT_SheetProtection.hashValue = XmlHelper.ReadString(node.Attributes["hashValue"]);
			cT_SheetProtection.saltValue = XmlHelper.ReadString(node.Attributes["saltValue"]);
			cT_SheetProtection.spinCount = XmlHelper.ReadString(node.Attributes["spinCount"]);
		}
		return cT_SheetProtection;
	}

	internal bool IsSetObjects()
	{
		return objects;
	}

	internal bool IsSetScenarios()
	{
		return scenarios;
	}

	internal bool IsSetSheet()
	{
		return sheet;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		if (algorithmName != null)
		{
			XmlHelper.WriteAttribute(sw, "algorithmName", algorithmName);
			XmlHelper.WriteAttribute(sw, "hashValue", hashValue);
			XmlHelper.WriteAttribute(sw, "saltValue", saltValue);
			XmlHelper.WriteAttribute(sw, "spinCount", spinCount);
		}
		XmlHelper.WriteAttribute(sw, "password", password);
		XmlHelper.WriteAttribute(sw, "sheet", sheet);
		if (objects)
		{
			XmlHelper.WriteAttribute(sw, "objects", objects);
		}
		if (scenarios)
		{
			XmlHelper.WriteAttribute(sw, "scenarios", scenarios);
		}
		if (!formatCells)
		{
			XmlHelper.WriteAttribute(sw, "formatCells", formatCells);
		}
		if (!formatColumns)
		{
			XmlHelper.WriteAttribute(sw, "formatColumns", formatColumns);
		}
		if (!formatRows)
		{
			XmlHelper.WriteAttribute(sw, "formatRows", formatRows);
		}
		if (!insertColumns)
		{
			XmlHelper.WriteAttribute(sw, "insertColumns", insertColumns);
		}
		if (!insertRows)
		{
			XmlHelper.WriteAttribute(sw, "insertRows", insertRows);
		}
		if (!insertHyperlinks)
		{
			XmlHelper.WriteAttribute(sw, "insertHyperlinks", insertHyperlinks);
		}
		if (!deleteColumns)
		{
			XmlHelper.WriteAttribute(sw, "deleteColumns", deleteColumns);
		}
		if (!deleteRows)
		{
			XmlHelper.WriteAttribute(sw, "deleteRows", deleteRows);
		}
		XmlHelper.WriteAttribute(sw, "selectLockedCells", selectLockedCells, writeIfBlank: false);
		if (!sort)
		{
			XmlHelper.WriteAttribute(sw, "sort", sort);
		}
		if (!autoFilter)
		{
			XmlHelper.WriteAttribute(sw, "autoFilter", autoFilter);
		}
		if (!pivotTables)
		{
			XmlHelper.WriteAttribute(sw, "pivotTables", pivotTables);
		}
		XmlHelper.WriteAttribute(sw, "selectUnlockedCells", selectUnlockedCells, writeIfBlank: false);
		sw.Write("/>");
	}
}
