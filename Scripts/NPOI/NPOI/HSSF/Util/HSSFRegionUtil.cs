using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.Util;

public class HSSFRegionUtil
{
	private class CellPropertySetter
	{
		private HSSFWorkbook _workbook;

		private string _propertyName;

		private short _propertyValue;

		public CellPropertySetter(HSSFWorkbook workbook, string propertyName, int value)
		{
			_workbook = workbook;
			_propertyName = propertyName;
			_propertyValue = (short)value;
		}

		public void SetProperty(IRow row, int column)
		{
			HSSFCellUtil.SetCellStyleProperty(HSSFCellUtil.GetCell(row, column), _workbook, _propertyName, _propertyValue);
		}
	}

	private HSSFRegionUtil()
	{
	}

	public static void SetBorderLeft(BorderStyle border, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int firstColumn = region.FirstColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "borderLeft", (int)border);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(HSSFCellUtil.GetRow(i, sheet), firstColumn);
		}
	}

	public static void SetLeftBorderColor(int color, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int firstColumn = region.FirstColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "leftBorderColor", color);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(HSSFCellUtil.GetRow(i, sheet), firstColumn);
		}
	}

	public static void SetBorderRight(BorderStyle border, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int lastColumn = region.LastColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "borderRight", (int)border);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(HSSFCellUtil.GetRow(i, sheet), lastColumn);
		}
	}

	public static void SetRightBorderColor(int color, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int lastColumn = region.LastColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "rightBorderColor", color);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(HSSFCellUtil.GetRow(i, sheet), lastColumn);
		}
	}

	public static void SetBorderBottom(BorderStyle border, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int lastRow = region.LastRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "borderBottom", (int)border);
		IRow row = HSSFCellUtil.GetRow(lastRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}

	public static void SetBottomBorderColor(int color, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int lastRow = region.LastRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "bottomBorderColor", color);
		IRow row = HSSFCellUtil.GetRow(lastRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}

	public static void SetBorderTop(BorderStyle border, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int firstRow = region.FirstRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "borderTop", (int)border);
		IRow row = HSSFCellUtil.GetRow(firstRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}

	public static void SetTopBorderColor(int color, CellRangeAddress region, HSSFSheet sheet, HSSFWorkbook workbook)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int firstRow = region.FirstRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter(workbook, "topBorderColor", color);
		IRow row = HSSFCellUtil.GetRow(firstRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}
}
