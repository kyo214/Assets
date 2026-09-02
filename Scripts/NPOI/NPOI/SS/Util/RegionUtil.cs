using NPOI.SS.UserModel;

namespace NPOI.SS.Util;

public class RegionUtil
{
	private class CellPropertySetter
	{
		private string _propertyName;

		private short _propertyValue;

		public CellPropertySetter(string propertyName, int value)
		{
			_propertyName = propertyName;
			_propertyValue = (short)value;
		}

		public void SetProperty(IRow row, int column)
		{
			CellUtil.SetCellStyleProperty(CellUtil.GetCell(row, column), _propertyName, _propertyValue);
		}
	}

	private RegionUtil()
	{
	}

	public static void SetBorderLeft(int border, CellRangeAddress region, ISheet sheet)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int firstColumn = region.FirstColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("borderLeft", border);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(CellUtil.GetRow(i, sheet), firstColumn);
		}
	}

	public static void SetLeftBorderColor(int color, CellRangeAddress region, ISheet sheet)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int firstColumn = region.FirstColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("leftBorderColor", color);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(CellUtil.GetRow(i, sheet), firstColumn);
		}
	}

	public static void SetBorderRight(int border, CellRangeAddress region, ISheet sheet)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int lastColumn = region.LastColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("borderRight", border);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(CellUtil.GetRow(i, sheet), lastColumn);
		}
	}

	public static void SetRightBorderColor(int color, CellRangeAddress region, ISheet sheet)
	{
		int firstRow = region.FirstRow;
		int lastRow = region.LastRow;
		int lastColumn = region.LastColumn;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("rightBorderColor", color);
		for (int i = firstRow; i <= lastRow; i++)
		{
			cellPropertySetter.SetProperty(CellUtil.GetRow(i, sheet), lastColumn);
		}
	}

	public static void SetBorderBottom(int border, CellRangeAddress region, ISheet sheet)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int lastRow = region.LastRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("borderBottom", border);
		IRow row = CellUtil.GetRow(lastRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}

	public static void SetBottomBorderColor(int color, CellRangeAddress region, ISheet sheet)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int lastRow = region.LastRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("bottomBorderColor", color);
		IRow row = CellUtil.GetRow(lastRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}

	public static void SetBorderTop(int border, CellRangeAddress region, ISheet sheet)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int firstRow = region.FirstRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("borderTop", border);
		IRow row = CellUtil.GetRow(firstRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}

	public static void SetTopBorderColor(int color, CellRangeAddress region, ISheet sheet)
	{
		int firstColumn = region.FirstColumn;
		int lastColumn = region.LastColumn;
		int firstRow = region.FirstRow;
		CellPropertySetter cellPropertySetter = new CellPropertySetter("topBorderColor", color);
		IRow row = CellUtil.GetRow(firstRow, sheet);
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			cellPropertySetter.SetProperty(row, i);
		}
	}
}
