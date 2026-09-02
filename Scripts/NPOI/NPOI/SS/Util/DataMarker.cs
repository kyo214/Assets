using NPOI.SS.UserModel;

namespace NPOI.SS.Util;

public class DataMarker
{
	private ISheet sheet;

	private CellRangeAddress range;

	public ISheet Sheet
	{
		get
		{
			return sheet;
		}
		set
		{
			sheet = value;
		}
	}

	public CellRangeAddress Range
	{
		get
		{
			return range;
		}
		set
		{
			range = value;
		}
	}

	public DataMarker(ISheet sheet, CellRangeAddress range)
	{
		this.sheet = sheet;
		this.range = range;
	}

	public string FormatAsString()
	{
		string sheetName = ((sheet == null) ? null : sheet.SheetName);
		if (range == null)
		{
			return null;
		}
		return range.FormatAsString(sheetName, useAbsoluteAddress: true);
	}
}
