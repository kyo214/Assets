using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula;
using NPOI.SS.Util;

namespace NPOI.SS.UserModel.Helpers;

public abstract class RowShifter
{
	protected ISheet sheet;

	public RowShifter(ISheet sh)
	{
		sheet = sh;
	}

	public List<CellRangeAddress> ShiftMergedRegions(int startRow, int endRow, int n)
	{
		List<CellRangeAddress> list = new List<CellRangeAddress>();
		ISet<int> set = new HashSet<int>();
		int numMergedRegions = sheet.NumMergedRegions;
		for (int i = 0; i < numMergedRegions; i++)
		{
			CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);
			if (startRow + n <= mergedRegion.FirstRow && endRow + n >= mergedRegion.LastRow)
			{
				set.Add(i);
				continue;
			}
			bool num = mergedRegion.FirstRow >= startRow || mergedRegion.LastRow >= startRow;
			bool flag = mergedRegion.FirstRow <= endRow || mergedRegion.LastRow <= endRow;
			if (num && flag && !mergedRegion.ContainsRow(startRow - 1) && !mergedRegion.ContainsRow(endRow + 1))
			{
				mergedRegion.FirstRow += n;
				mergedRegion.LastRow += n;
				list.Add(mergedRegion);
				set.Add(i);
			}
		}
		if (set.Count != 0)
		{
			sheet.RemoveMergedRegions(set.ToList());
		}
		foreach (CellRangeAddress item in list)
		{
			sheet.AddMergedRegion(item);
		}
		return list;
	}

	public abstract void UpdateNamedRanges(FormulaShifter Shifter);

	public abstract void UpdateFormulas(FormulaShifter Shifter);

	public abstract void UpdateRowFormulas(IRow row, FormulaShifter Shifter);

	public abstract void UpdateConditionalFormatting(FormulaShifter Shifter);

	public abstract void UpdateHyperlinks(FormulaShifter Shifter);
}
