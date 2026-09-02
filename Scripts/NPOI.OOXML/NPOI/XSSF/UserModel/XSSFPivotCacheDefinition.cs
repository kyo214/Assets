using System;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFPivotCacheDefinition : POIXMLDocumentPart
{
	private CT_PivotCacheDefinition ctPivotCacheDefinition;

	public XSSFPivotCacheDefinition()
	{
		ctPivotCacheDefinition = new CT_PivotCacheDefinition();
		CreateDefaultValues();
	}

	protected XSSFPivotCacheDefinition(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFPivotCacheDefinition(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			XmlDocument xmlDocument = POIXMLDocumentPart.ConvertStreamToXml(is1);
			ctPivotCacheDefinition = CT_PivotCacheDefinition.Parse(xmlDocument.DocumentElement, POIXMLDocumentPart.NamespaceManager);
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public CT_PivotCacheDefinition GetCTPivotCacheDefinition()
	{
		return ctPivotCacheDefinition;
	}

	private void CreateDefaultValues()
	{
		ctPivotCacheDefinition.createdVersion = (byte)XSSFPivotTable.CREATED_VERSION;
		ctPivotCacheDefinition.minRefreshableVersion = (byte)XSSFPivotTable.MIN_REFRESHABLE_VERSION;
		ctPivotCacheDefinition.refreshedVersion = (byte)XSSFPivotTable.UPDATED_VERSION;
		ctPivotCacheDefinition.refreshedBy = "NPOI";
		ctPivotCacheDefinition.refreshedDate = DateTime.Now.ToOADate();
		ctPivotCacheDefinition.refreshOnLoad = true;
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		ctPivotCacheDefinition.Save(outputStream);
		outputStream.Close();
	}

	public AreaReference GetPivotArea(IWorkbook wb)
	{
		CT_WorksheetSource worksheetSource = ctPivotCacheDefinition.cacheSource.worksheetSource;
		string text = worksheetSource.@ref;
		string name = worksheetSource.name;
		if (text == null && name == null)
		{
			throw new ArgumentException("Pivot cache must reference an area, named range, or table.");
		}
		if (text != null)
		{
			return new AreaReference(text, SpreadsheetVersion.EXCEL2007);
		}
		if (name != null)
		{
			IName name2 = wb.GetName(name);
			if (name2 != null)
			{
				return new AreaReference(name2.RefersToFormula, SpreadsheetVersion.EXCEL2007);
			}
			foreach (XSSFTable table in ((XSSFSheet)wb.GetSheet(worksheetSource.sheet)).GetTables())
			{
				if (table.Name.Equals(name))
				{
					return new AreaReference(table.StartCellReference, table.EndCellReference);
				}
			}
		}
		throw new ArgumentException("Name '" + name + "' was not found.");
	}

	protected internal void CreateCacheFields(ISheet sheet)
	{
		AreaReference pivotArea = GetPivotArea(sheet.Workbook);
		CellReference firstCell = pivotArea.FirstCell;
		CellReference lastCell = pivotArea.LastCell;
		int col = firstCell.Col;
		int col2 = lastCell.Col;
		IRow row = sheet.GetRow(firstCell.Row);
		CT_CacheFields cT_CacheFields = ((ctPivotCacheDefinition.cacheFields == null) ? ctPivotCacheDefinition.AddNewCacheFields() : ctPivotCacheDefinition.cacheFields);
		for (int i = col; i <= col2; i++)
		{
			CT_CacheField cT_CacheField = cT_CacheFields.AddNewCacheField();
			if (i == col2)
			{
				cT_CacheFields.count = cT_CacheFields.SizeOfCacheFieldArray();
			}
			cT_CacheField.numFmtId = 0u;
			ICell cell = row.GetCell(i);
			cell.SetCellType(CellType.String);
			string stringCellValue = cell.StringCellValue;
			cT_CacheField.name = stringCellValue;
			cT_CacheField.AddNewSharedItems();
		}
	}
}
