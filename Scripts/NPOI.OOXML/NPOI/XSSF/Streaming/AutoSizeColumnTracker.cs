using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.Streaming;

public class AutoSizeColumnTracker
{
	private class ColumnWidthPair
	{
		private double withSkipMergedCells;

		private double withUseMergedCells;

		public ColumnWidthPair()
			: this(-1.0, -1.0)
		{
		}

		public ColumnWidthPair(double columnWidthSkipMergedCells, double columnWidthUseMergedCells)
		{
			withSkipMergedCells = columnWidthSkipMergedCells;
			withUseMergedCells = columnWidthUseMergedCells;
		}

		public double GetMaxColumnWidth(bool useMergedCells)
		{
			if (!useMergedCells)
			{
				return withSkipMergedCells;
			}
			return withUseMergedCells;
		}

		public void SetMaxColumnWidths(double unmergedWidth, double mergedWidth)
		{
			withUseMergedCells = Math.Max(withUseMergedCells, mergedWidth);
			withSkipMergedCells = Math.Max(withUseMergedCells, unmergedWidth);
		}
	}

	private int defaultCharWidth;

	private DataFormatter dataFormatter = new DataFormatter();

	private Dictionary<int, ColumnWidthPair> maxColumnWidths = new Dictionary<int, ColumnWidthPair>();

	private HashSet<int> untrackedColumns = new HashSet<int>();

	private bool trackAllColumns;

	public ISet<int> TrackedColumns => new SortedSet<int>(maxColumnWidths.Keys);

	public AutoSizeColumnTracker(ISheet sheet)
	{
		defaultCharWidth = SheetUtil.GetDefaultCharWidth(sheet.Workbook);
	}

	public bool IsColumnTracked(int column)
	{
		if (!trackAllColumns)
		{
			return maxColumnWidths.ContainsKey(column);
		}
		return true;
	}

	public bool IsAllColumnsTracked()
	{
		return trackAllColumns;
	}

	public void TrackAllColumns()
	{
		trackAllColumns = true;
		untrackedColumns.Clear();
	}

	public void UntrackAllColumns()
	{
		trackAllColumns = false;
		maxColumnWidths.Clear();
		untrackedColumns.Clear();
	}

	public void TrackColumns(ICollection<int> columns)
	{
		foreach (int column in columns)
		{
			TrackColumn(column);
		}
	}

	public bool TrackColumn(int column)
	{
		untrackedColumns.Remove(column);
		if (!maxColumnWidths.ContainsKey(column))
		{
			maxColumnWidths.Add(column, new ColumnWidthPair());
			return true;
		}
		return false;
	}

	private bool ImplicitlyTrackColumn(int column)
	{
		if (!untrackedColumns.Contains(column))
		{
			TrackColumn(column);
			return true;
		}
		return false;
	}

	public bool UntrackColumns(ICollection<int> columns)
	{
		bool result = false;
		foreach (int column in columns)
		{
			untrackedColumns.Add(column);
			if (maxColumnWidths.ContainsKey(column))
			{
				result = maxColumnWidths.Remove(column);
			}
		}
		return result;
	}

	public bool UntrackColumn(int column)
	{
		bool result = false;
		if (maxColumnWidths.ContainsKey(column))
		{
			untrackedColumns.Add(column);
			result = maxColumnWidths.Remove(column);
		}
		untrackedColumns.Add(column);
		return result;
	}

	public int GetBestFitColumnWidth(int column, bool useMergedCells)
	{
		if (!maxColumnWidths.ContainsKey(column))
		{
			if (!trackAllColumns)
			{
				InvalidOperationException innerException = new InvalidOperationException("Column was never explicitly tracked and isAllColumnsTracked() is false (trackAllColumns() was never called or untrackAllColumns() was called after trackAllColumns() was called).");
				throw new InvalidOperationException("Cannot get best fit column width on untracked column " + column + ". Either explicitly track the column or track all columns.", innerException);
			}
			if (!ImplicitlyTrackColumn(column))
			{
				InvalidOperationException innerException2 = new InvalidOperationException("Column was explicitly untracked after trackAllColumns() was called.");
				throw new InvalidOperationException("Cannot get best fit column width on explicitly untracked column " + column + ". Either explicitly track the column or track all columns.", innerException2);
			}
		}
		double maxColumnWidth = maxColumnWidths[column].GetMaxColumnWidth(useMergedCells);
		return (int)(256.0 * maxColumnWidth);
	}

	public void UpdateColumnWidths(IRow row)
	{
		ImplicitlyTrackColumnsInRow(row);
		if (maxColumnWidths.Count < row.PhysicalNumberOfCells)
		{
			foreach (KeyValuePair<int, ColumnWidthPair> maxColumnWidth in maxColumnWidths)
			{
				int key = maxColumnWidth.Key;
				ICell cell = row.GetCell(key);
				if (cell != null)
				{
					ColumnWidthPair value = maxColumnWidth.Value;
					UpdateColumnWidth(cell, value);
				}
			}
			return;
		}
		foreach (ICell item in row)
		{
			int columnIndex = item.ColumnIndex;
			if (maxColumnWidths.ContainsKey(columnIndex))
			{
				ColumnWidthPair pair = maxColumnWidths[columnIndex];
				UpdateColumnWidth(item, pair);
			}
		}
	}

	private void ImplicitlyTrackColumnsInRow(IRow row)
	{
		if (!trackAllColumns)
		{
			return;
		}
		foreach (ICell item in row)
		{
			int columnIndex = item.ColumnIndex;
			ImplicitlyTrackColumn(columnIndex);
		}
	}

	private void UpdateColumnWidth(ICell cell, ColumnWidthPair pair)
	{
		double cellWidth = SheetUtil.GetCellWidth(cell, defaultCharWidth, dataFormatter, useMergedCells: false);
		double cellWidth2 = SheetUtil.GetCellWidth(cell, defaultCharWidth, dataFormatter, useMergedCells: true);
		pair.SetMaxColumnWidths(cellWidth, cellWidth2);
	}
}
