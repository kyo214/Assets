using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.UserModel;

public class XSSFTable : POIXMLDocumentPart, ITable
{
	private CT_Table ctTable;

	private List<XSSFXmlColumnPr> xmlColumnPr;

	private CT_TableColumn[] ctColumns;

	private Dictionary<string, int> columnMap;

	private CellReference startCellReference;

	private CellReference endCellReference;

	private string commonXPath;

	private CT_TableColumn[] TableColumns
	{
		get
		{
			if (ctColumns == null)
			{
				ctColumns = ctTable.tableColumns.tableColumn.ToArray();
			}
			return ctColumns;
		}
	}

	public string Name
	{
		get
		{
			return ctTable.name;
		}
		set
		{
			ctTable.name = value;
		}
	}

	public string DisplayName
	{
		get
		{
			return ctTable.displayName;
		}
		set
		{
			ctTable.displayName = value;
		}
	}

	public long NumberOfMappedColumns => ctTable.tableColumns.count;

	public CellReference StartCellReference
	{
		get
		{
			if (startCellReference == null)
			{
				SetCellReferences();
			}
			return startCellReference;
		}
	}

	public CellReference EndCellReference
	{
		get
		{
			if (endCellReference == null)
			{
				SetCellReferences();
			}
			return endCellReference;
		}
	}

	public int RowCount
	{
		get
		{
			CellReference cellReference = StartCellReference;
			CellReference cellReference2 = EndCellReference;
			int result = 0;
			if (cellReference != null && cellReference2 != null)
			{
				result = cellReference2.Row - cellReference.Row + 1;
			}
			return result;
		}
	}

	public string SheetName => GetXSSFSheet().SheetName;

	public bool IsHasTotalsRow => ctTable.totalsRowShown;

	public int StartColIndex => StartCellReference.Col;

	public int StartRowIndex => StartCellReference.Row;

	public int EndColIndex => EndCellReference.Col;

	public int EndRowIndex => EndCellReference.Row;

	public XSSFTable()
	{
		ctTable = new CT_Table();
	}

	internal XSSFTable(PackagePart part)
		: base(part)
	{
		XmlDocument xmlDoc = POIXMLDocumentPart.ConvertStreamToXml(part.GetInputStream());
		ReadFrom(xmlDoc);
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(XmlDocument xmlDoc)
	{
		try
		{
			TableDocument tableDocument = TableDocument.Parse(xmlDoc, POIXMLDocumentPart.NamespaceManager);
			ctTable = tableDocument.GetTable();
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public XSSFSheet GetXSSFSheet()
	{
		return (XSSFSheet)GetParent();
	}

	public void WriteTo(Stream out1)
	{
		UpdateHeaders();
		TableDocument tableDocument = new TableDocument();
		tableDocument.SetTable(ctTable);
		tableDocument.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}

	public CT_Table GetCTTable()
	{
		return ctTable;
	}

	public bool MapsTo(long id)
	{
		bool result = false;
		foreach (XSSFXmlColumnPr xmlColumnPr in GetXmlColumnPrs())
		{
			if (xmlColumnPr.GetMapId() == id)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public string GetCommonXpath()
	{
		if (commonXPath == null)
		{
			Array array = null;
			CT_TableColumn[] tableColumns = TableColumns;
			foreach (CT_TableColumn cT_TableColumn in tableColumns)
			{
				if (cT_TableColumn.xmlColumnPr == null)
				{
					continue;
				}
				string[] array2 = cT_TableColumn.xmlColumnPr.xpath.Split(new char[1] { '/' });
				if (array == null)
				{
					array = array2;
					continue;
				}
				int num = ((array.Length > array2.Length) ? array2.Length : array.Length);
				for (int j = 0; j < num; j++)
				{
					if (!array.GetValue(j).Equals(array2[j]))
					{
						array = Arrays.AsList(array).GetRange(0, j).ToArray(typeof(string));
						break;
					}
				}
			}
			commonXPath = "";
			for (int k = 1; k < array.Length; k++)
			{
				commonXPath = commonXPath + "/" + array.GetValue(k);
			}
		}
		return commonXPath;
	}

	public List<XSSFXmlColumnPr> GetXmlColumnPrs()
	{
		if (xmlColumnPr == null)
		{
			xmlColumnPr = new List<XSSFXmlColumnPr>();
			foreach (CT_TableColumn item2 in ctTable.tableColumns.tableColumn)
			{
				if (item2.xmlColumnPr != null)
				{
					XSSFXmlColumnPr item = new XSSFXmlColumnPr(this, item2, item2.xmlColumnPr);
					xmlColumnPr.Add(item);
				}
			}
		}
		return xmlColumnPr;
	}

	private void SetCellReferences()
	{
		string text = ctTable.@ref;
		if (text != null)
		{
			string[] array = text.Split(new char[1] { ':' }, 2);
			string cellRef = array[0];
			string cellRef2 = array[1];
			startCellReference = new CellReference(cellRef);
			endCellReference = new CellReference(cellRef2);
		}
	}

	public void UpdateReferences()
	{
		startCellReference = null;
		endCellReference = null;
	}

	public void UpdateHeaders()
	{
		XSSFSheet xSSFSheet = (XSSFSheet)GetParent();
		CellReference cellReference = StartCellReference;
		if (cellReference == null)
		{
			return;
		}
		int row = cellReference.Row;
		int col = cellReference.Col;
		if (!(xSSFSheet.GetRow(row) is XSSFRow xSSFRow) || xSSFRow.GetCTRow() == null)
		{
			return;
		}
		int num = col;
		foreach (CT_TableColumn item in GetCTTable().tableColumns.tableColumn)
		{
			if (xSSFRow.GetCell(num) is XSSFCell xSSFCell)
			{
				item.name = xSSFCell.StringCellValue;
			}
			num++;
		}
		ctColumns = null;
		columnMap = null;
	}

	public int FindColumnIndex(string columnHeader)
	{
		if (columnHeader == null)
		{
			return -1;
		}
		if (columnMap == null)
		{
			columnMap = new Dictionary<string, int>(TableColumns.Length * 3 / 2);
			int num = 0;
			CT_TableColumn[] tableColumns = TableColumns;
			foreach (CT_TableColumn cT_TableColumn in tableColumns)
			{
				columnMap.Add(cT_TableColumn.name.ToUpper(CultureInfo.CurrentCulture), num);
				num++;
			}
		}
		int result = -1;
		string key = columnHeader.Replace("'", "").ToUpper(CultureInfo.CurrentCulture);
		if (columnMap.ContainsKey(key))
		{
			result = columnMap[key];
		}
		return result;
	}
}
