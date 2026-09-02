using System;
using System.Collections;
using System.Collections.Generic;

namespace NPOI.SS.Util;

public class CellRangeUtil
{
	public const int NO_INTERSECTION = 1;

	public const int OVERLAP = 2;

	public const int INSIDE = 3;

	public const int ENCLOSES = 4;

	private CellRangeUtil()
	{
	}

	public static int Intersect(CellRangeAddress crA, CellRangeAddress crB)
	{
		int firstRow = crB.FirstRow;
		int lastRow = crB.LastRow;
		int firstColumn = crB.FirstColumn;
		int lastColumn = crB.LastColumn;
		if (gt(crA.FirstRow, lastRow) || lt(crA.LastRow, firstRow) || gt(crA.FirstColumn, lastColumn) || lt(crA.LastColumn, firstColumn))
		{
			return 1;
		}
		if (Contains(crA, crB))
		{
			return 3;
		}
		if (Contains(crB, crA))
		{
			return 4;
		}
		return 2;
	}

	public static CellRangeAddress[] MergeCellRanges(CellRangeAddress[] cellRanges)
	{
		if (cellRanges.Length < 1)
		{
			return new CellRangeAddress[0];
		}
		return MergeCellRanges(new List<CellRangeAddress>(cellRanges)).ToArray();
	}

	private static List<CellRangeAddress> MergeCellRanges(List<CellRangeAddress> cellRangeList)
	{
		while (cellRangeList.Count > 1)
		{
			bool flag = false;
			for (int i = 0; i < cellRangeList.Count; i++)
			{
				CellRangeAddress range = cellRangeList[i];
				for (int j = i + 1; j < cellRangeList.Count; j++)
				{
					CellRangeAddress range2 = cellRangeList[j];
					CellRangeAddress[] array = MergeRanges(range, range2);
					if (array != null)
					{
						flag = true;
						cellRangeList[i] = array[0];
						cellRangeList.RemoveAt(j--);
						for (int k = 1; k < array.Length; k++)
						{
							j++;
							cellRangeList.Insert(j, array[k]);
						}
					}
				}
			}
			if (!flag)
			{
				break;
			}
		}
		return cellRangeList;
	}

	private static CellRangeAddress[] MergeRanges(CellRangeAddress range1, CellRangeAddress range2)
	{
		int num = Intersect(range1, range2);
		switch (num)
		{
		case 1:
			if (HasExactSharedBorder(range1, range2))
			{
				return new CellRangeAddress[1] { CreateEnclosingCellRange(range1, range2) };
			}
			return null;
		case 2:
			return null;
		case 3:
			return new CellRangeAddress[1] { range1 };
		case 4:
			return new CellRangeAddress[1] { range2 };
		default:
			throw new InvalidOperationException("unexpected intersection result (" + num + ")");
		}
	}

	[Obsolete]
	private static CellRangeAddress[] ToArray(ArrayList temp)
	{
		_ = new CellRangeAddress[temp.Count];
		return (CellRangeAddress[])temp.ToArray(typeof(CellRangeAddress));
	}

	public static bool Contains(CellRangeAddress crA, CellRangeAddress crB)
	{
		if (le(crA.FirstRow, crB.FirstRow) && ge(crA.LastRow, crB.LastRow) && le(crA.FirstColumn, crB.FirstColumn))
		{
			return ge(crA.LastColumn, crB.LastColumn);
		}
		return false;
	}

	public static bool HasExactSharedBorder(CellRangeAddress crA, CellRangeAddress crB)
	{
		int firstRow = crB.FirstRow;
		int lastRow = crB.LastRow;
		int firstColumn = crB.FirstColumn;
		int lastColumn = crB.LastColumn;
		if ((crA.FirstRow > 0 && crA.FirstRow - 1 == lastRow) || (firstRow > 0 && firstRow - 1 == crA.LastRow))
		{
			if (crA.FirstColumn == firstColumn)
			{
				return crA.LastColumn == lastColumn;
			}
			return false;
		}
		if ((crA.FirstColumn > 0 && crA.FirstColumn - 1 == lastColumn) || (firstColumn > 0 && crA.LastColumn == firstColumn - 1))
		{
			if (crA.FirstRow == firstRow)
			{
				return crA.LastRow == lastRow;
			}
			return false;
		}
		return false;
	}

	public static CellRangeAddress CreateEnclosingCellRange(CellRangeAddress crA, CellRangeAddress crB)
	{
		if (crB == null)
		{
			return crA.Copy();
		}
		int firstRow = (lt(crB.FirstRow, crA.FirstRow) ? crB.FirstRow : crA.FirstRow);
		int lastRow = (gt(crB.LastRow, crA.LastRow) ? crB.LastRow : crA.LastRow);
		int firstCol = (lt(crB.FirstColumn, crA.FirstColumn) ? crB.FirstColumn : crA.FirstColumn);
		int lastCol = (gt(crB.LastColumn, crA.LastColumn) ? crB.LastColumn : crA.LastColumn);
		return new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
	}

	private static bool lt(int a, int b)
	{
		if (a != -1)
		{
			if (b != -1)
			{
				return a < b;
			}
			return true;
		}
		return false;
	}

	private static bool le(int a, int b)
	{
		if (a != b)
		{
			return lt(a, b);
		}
		return true;
	}

	private static bool gt(int a, int b)
	{
		return lt(b, a);
	}

	private static bool ge(int a, int b)
	{
		return !lt(a, b);
	}
}
