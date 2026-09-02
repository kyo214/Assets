using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NPOI.HSSF.Model;

namespace NPOI.HSSF.Record.Aggregates;

public class ColumnInfoRecordsAggregate : RecordAggregate, ICloneable
{
	private class CIRComparator : IComparer<ColumnInfoRecord>
	{
		public static IComparer<ColumnInfoRecord> instance = new CIRComparator();

		private CIRComparator()
		{
		}

		public static int CompareColInfos(ColumnInfoRecord a, ColumnInfoRecord b)
		{
			return a.FirstColumn - b.FirstColumn;
		}

		public int Compare(ColumnInfoRecord x, ColumnInfoRecord y)
		{
			return CompareColInfos(x, y);
		}
	}

	private List<ColumnInfoRecord> records;

	public override short Sid => -1012;

	public int NumColumns => records.Count;

	public override int RecordSize
	{
		get
		{
			int num = 0;
			IEnumerator enumerator = records.GetEnumerator();
			while (enumerator.MoveNext())
			{
				num += ((ColumnInfoRecord)enumerator.Current).RecordSize;
			}
			return num;
		}
	}

	public int MaxOutlineLevel
	{
		get
		{
			int num = 0;
			int count = records.Count;
			for (int i = 0; i < count; i++)
			{
				num = Math.Max(GetColInfo(i).OutlineLevel, num);
			}
			return num;
		}
	}

	public ColumnInfoRecordsAggregate()
	{
		records = new List<ColumnInfoRecord>();
	}

	public ColumnInfoRecordsAggregate(RecordStream rs)
		: this()
	{
		bool flag = true;
		ColumnInfoRecord columnInfoRecord = null;
		while (rs.PeekNextClass() == typeof(ColumnInfoRecord))
		{
			ColumnInfoRecord columnInfoRecord2 = (ColumnInfoRecord)rs.GetNext();
			records.Add(columnInfoRecord2);
			if (columnInfoRecord != null && CIRComparator.CompareColInfos(columnInfoRecord, columnInfoRecord2) > 0)
			{
				flag = false;
			}
			columnInfoRecord = columnInfoRecord2;
		}
		if (records.Count < 1)
		{
			throw new InvalidOperationException("No column info records found");
		}
		if (!flag)
		{
			records.Sort(CIRComparator.instance);
		}
	}

	public IEnumerator GetEnumerator()
	{
		return records.GetEnumerator();
	}

	public object Clone()
	{
		ColumnInfoRecordsAggregate columnInfoRecordsAggregate = new ColumnInfoRecordsAggregate();
		for (int i = 0; i < records.Count; i++)
		{
			ColumnInfoRecord columnInfoRecord = records[i];
			columnInfoRecord = (ColumnInfoRecord)columnInfoRecord.Clone();
			columnInfoRecordsAggregate.records.Add(columnInfoRecord);
		}
		return columnInfoRecordsAggregate;
	}

	public void InsertColumn(ColumnInfoRecord col)
	{
		records.Add(col);
		records.Sort(CIRComparator.instance);
	}

	public void InsertColumn(int idx, ColumnInfoRecord col)
	{
		records.Insert(idx, col);
	}

	public override int Serialize(int offset, byte[] data)
	{
		IEnumerator enumerator = records.GetEnumerator();
		int num = offset;
		while (enumerator.MoveNext())
		{
			num += ((Record)enumerator.Current).Serialize(num, data);
		}
		return num - offset;
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		int count = records.Count;
		if (count < 1)
		{
			return;
		}
		ColumnInfoRecord columnInfoRecord = null;
		for (int i = 0; i < count; i++)
		{
			ColumnInfoRecord columnInfoRecord2 = records[i];
			rv.VisitRecord(columnInfoRecord2);
			if (columnInfoRecord != null && CIRComparator.CompareColInfos(columnInfoRecord, columnInfoRecord2) > 0)
			{
				throw new InvalidOperationException("Column info records are out of order");
			}
			columnInfoRecord = columnInfoRecord2;
		}
	}

	public int FindStartOfColumnOutlineGroup(int idx)
	{
		ColumnInfoRecord columnInfoRecord = records[idx];
		int outlineLevel = columnInfoRecord.OutlineLevel;
		while (idx != 0)
		{
			ColumnInfoRecord columnInfoRecord2 = records[idx - 1];
			if (columnInfoRecord.FirstColumn - 1 != columnInfoRecord2.LastColumn || columnInfoRecord2.OutlineLevel < outlineLevel)
			{
				break;
			}
			idx--;
			columnInfoRecord = columnInfoRecord2;
		}
		return idx;
	}

	public int FindEndOfColumnOutlineGroup(int idx)
	{
		ColumnInfoRecord columnInfoRecord = records[idx];
		int outlineLevel = columnInfoRecord.OutlineLevel;
		while (idx < records.Count - 1)
		{
			ColumnInfoRecord columnInfoRecord2 = records[idx + 1];
			if (columnInfoRecord.LastColumn + 1 != columnInfoRecord2.FirstColumn || columnInfoRecord2.OutlineLevel < outlineLevel)
			{
				break;
			}
			idx++;
			columnInfoRecord = columnInfoRecord2;
		}
		return idx;
	}

	public ColumnInfoRecord GetColInfo(int idx)
	{
		return records[idx];
	}

	public bool IsColumnGroupCollapsed(int idx)
	{
		int num = FindEndOfColumnOutlineGroup(idx);
		int num2 = num + 1;
		if (num2 >= records.Count)
		{
			return false;
		}
		ColumnInfoRecord colInfo = GetColInfo(num2);
		if (!GetColInfo(num).IsAdjacentBefore(colInfo))
		{
			return false;
		}
		return colInfo.IsCollapsed;
	}

	public bool IsColumnGroupHiddenByParent(int idx)
	{
		int num = 0;
		bool result = false;
		int num2 = FindEndOfColumnOutlineGroup(idx);
		if (num2 < records.Count)
		{
			ColumnInfoRecord colInfo = GetColInfo(num2 + 1);
			if (GetColInfo(num2).IsAdjacentBefore(colInfo))
			{
				num = colInfo.OutlineLevel;
				result = colInfo.IsHidden;
			}
		}
		int num3 = 0;
		bool result2 = false;
		int num4 = FindStartOfColumnOutlineGroup(idx);
		if (num4 > 0)
		{
			ColumnInfoRecord colInfo2 = GetColInfo(num4 - 1);
			if (colInfo2.IsAdjacentBefore(GetColInfo(num4)))
			{
				num3 = colInfo2.OutlineLevel;
				result2 = colInfo2.IsHidden;
			}
		}
		if (num > num3)
		{
			return result;
		}
		return result2;
	}

	public void CollapseColumn(int columnNumber)
	{
		int num = FindColInfoIdx(columnNumber, 0);
		if (num != -1)
		{
			int num2 = FindStartOfColumnOutlineGroup(num);
			ColumnInfoRecord colInfo = GetColInfo(num2);
			int num3 = SetGroupHidden(num2, colInfo.OutlineLevel, hidden: true);
			SetColumn(num3 + 1, null, null, null, null, true);
		}
	}

	public void ExpandColumn(int columnNumber)
	{
		int num = FindColInfoIdx(columnNumber, 0);
		if (num == -1 || !IsColumnGroupCollapsed(num))
		{
			return;
		}
		int num2 = FindStartOfColumnOutlineGroup(num);
		ColumnInfoRecord colInfo = GetColInfo(num2);
		int num3 = FindEndOfColumnOutlineGroup(num);
		GetColInfo(num3);
		if (!IsColumnGroupHiddenByParent(num))
		{
			for (int i = num2; i <= num3; i++)
			{
				if (colInfo.OutlineLevel == GetColInfo(i).OutlineLevel)
				{
					GetColInfo(i).IsHidden = false;
				}
			}
		}
		SetColumn(colInfo.LastColumn + 1, null, null, null, null, false);
	}

	private static void SetColumnInfoFields(ColumnInfoRecord ci, short? xfStyle, int? width, int? level, bool? hidden, bool? collapsed)
	{
		if (xfStyle.HasValue)
		{
			ci.XFIndex = Convert.ToInt16(xfStyle, CultureInfo.InvariantCulture);
		}
		if (width.HasValue)
		{
			ci.ColumnWidth = Convert.ToInt32(width, CultureInfo.InvariantCulture);
		}
		if (level.HasValue)
		{
			ci.OutlineLevel = (short)level.Value;
		}
		if (hidden.HasValue)
		{
			ci.IsHidden = Convert.ToBoolean(hidden, CultureInfo.InvariantCulture);
		}
		if (collapsed.HasValue)
		{
			ci.IsCollapsed = Convert.ToBoolean(collapsed, CultureInfo.InvariantCulture);
		}
	}

	private void AttemptMergeColInfoRecords(int colInfoIx)
	{
		int count = records.Count;
		if (colInfoIx < 0 || colInfoIx >= count)
		{
			throw new ArgumentException("colInfoIx " + colInfoIx + " is out of range (0.." + (count - 1) + ")");
		}
		ColumnInfoRecord colInfo = GetColInfo(colInfoIx);
		int num = colInfoIx + 1;
		if (num < count && MergeColInfoRecords(colInfo, GetColInfo(num)))
		{
			records.RemoveAt(num);
		}
		if (colInfoIx > 0 && MergeColInfoRecords(GetColInfo(colInfoIx - 1), colInfo))
		{
			records.RemoveAt(colInfoIx);
		}
	}

	private static bool MergeColInfoRecords(ColumnInfoRecord ciA, ColumnInfoRecord ciB)
	{
		if (ciA.IsAdjacentBefore(ciB) && ciA.FormatMatches(ciB))
		{
			ciA.LastColumn = ciB.LastColumn;
			return true;
		}
		return false;
	}

	private int SetGroupHidden(int pIdx, int level, bool hidden)
	{
		int i = pIdx;
		ColumnInfoRecord columnInfoRecord = GetColInfo(i);
		for (; i < records.Count; i++)
		{
			columnInfoRecord.IsHidden = hidden;
			if (i + 1 < records.Count)
			{
				ColumnInfoRecord colInfo = GetColInfo(i + 1);
				if (!columnInfoRecord.IsAdjacentBefore(colInfo) || colInfo.OutlineLevel < level)
				{
					break;
				}
				columnInfoRecord = colInfo;
			}
		}
		return columnInfoRecord.LastColumn;
	}

	public void SetColumn(int targetColumnIx, short? xfIndex, int? width, int? level, bool? hidden, bool? collapsed)
	{
		ColumnInfoRecord columnInfoRecord = null;
		int num = 0;
		for (num = 0; num < records.Count; num++)
		{
			ColumnInfoRecord columnInfoRecord2 = records[num];
			if (columnInfoRecord2.ContainsColumn(targetColumnIx))
			{
				columnInfoRecord = columnInfoRecord2;
				break;
			}
			if (columnInfoRecord2.FirstColumn > targetColumnIx)
			{
				break;
			}
		}
		if (columnInfoRecord == null)
		{
			ColumnInfoRecord columnInfoRecord3 = new ColumnInfoRecord();
			columnInfoRecord3.FirstColumn = targetColumnIx;
			columnInfoRecord3.LastColumn = targetColumnIx;
			SetColumnInfoFields(columnInfoRecord3, xfIndex, width, level, hidden, collapsed);
			InsertColumn(num, columnInfoRecord3);
			AttemptMergeColInfoRecords(num);
			return;
		}
		bool num2 = columnInfoRecord.XFIndex != xfIndex;
		bool flag = columnInfoRecord.ColumnWidth != width;
		bool flag2 = columnInfoRecord.OutlineLevel != level;
		bool flag3 = columnInfoRecord.IsHidden != hidden;
		bool flag4 = columnInfoRecord.IsCollapsed != collapsed;
		if (!(num2 | flag | flag2 | flag3 | flag4))
		{
			return;
		}
		if (columnInfoRecord.FirstColumn == targetColumnIx && columnInfoRecord.LastColumn == targetColumnIx)
		{
			SetColumnInfoFields(columnInfoRecord, xfIndex, width, level, hidden, collapsed);
			AttemptMergeColInfoRecords(num);
		}
		else if (columnInfoRecord.FirstColumn == targetColumnIx || columnInfoRecord.LastColumn == targetColumnIx)
		{
			if (columnInfoRecord.FirstColumn == targetColumnIx)
			{
				columnInfoRecord.FirstColumn = targetColumnIx + 1;
			}
			else
			{
				columnInfoRecord.LastColumn = targetColumnIx - 1;
				num++;
			}
			ColumnInfoRecord columnInfoRecord4 = CopyColInfo(columnInfoRecord);
			columnInfoRecord4.FirstColumn = targetColumnIx;
			columnInfoRecord4.LastColumn = targetColumnIx;
			SetColumnInfoFields(columnInfoRecord4, xfIndex, width, level, hidden, collapsed);
			InsertColumn(num, columnInfoRecord4);
			AttemptMergeColInfoRecords(num);
		}
		else
		{
			ColumnInfoRecord columnInfoRecord5 = columnInfoRecord;
			ColumnInfoRecord columnInfoRecord6 = CopyColInfo(columnInfoRecord);
			ColumnInfoRecord columnInfoRecord7 = CopyColInfo(columnInfoRecord);
			int lastColumn = columnInfoRecord.LastColumn;
			columnInfoRecord5.LastColumn = targetColumnIx - 1;
			columnInfoRecord6.FirstColumn = targetColumnIx;
			columnInfoRecord6.LastColumn = targetColumnIx;
			SetColumnInfoFields(columnInfoRecord6, xfIndex, width, level, hidden, collapsed);
			InsertColumn(++num, columnInfoRecord6);
			columnInfoRecord7.FirstColumn = targetColumnIx + 1;
			columnInfoRecord7.LastColumn = lastColumn;
			InsertColumn(++num, columnInfoRecord7);
		}
	}

	private ColumnInfoRecord CopyColInfo(ColumnInfoRecord ci)
	{
		return (ColumnInfoRecord)ci.Clone();
	}

	private void SetColumnInfoFields(ColumnInfoRecord ci, short xfStyle, short width, int level, bool hidden, bool collapsed)
	{
		ci.XFIndex = xfStyle;
		ci.ColumnWidth = width;
		ci.OutlineLevel = (short)level;
		ci.IsHidden = hidden;
		ci.IsCollapsed = collapsed;
	}

	[Obsolete("Not found in poi")]
	public void CollapseColInfoRecords(int columnIdx)
	{
		if (columnIdx != 0)
		{
			ColumnInfoRecord columnInfoRecord = records[columnIdx - 1];
			ColumnInfoRecord columnInfoRecord2 = records[columnIdx];
			if (columnInfoRecord.LastColumn == columnInfoRecord2.FirstColumn - 1 && columnInfoRecord.XFIndex == columnInfoRecord2.XFIndex && columnInfoRecord.Options == columnInfoRecord2.Options && columnInfoRecord.ColumnWidth == columnInfoRecord2.ColumnWidth)
			{
				columnInfoRecord.LastColumn = columnInfoRecord2.LastColumn;
				records.RemoveAt(columnIdx);
			}
		}
	}

	public void GroupColumnRange(int fromColumnIx, int toColumnIx, bool indent)
	{
		int fromColInfoIdx = 0;
		for (int i = fromColumnIx; i <= toColumnIx; i++)
		{
			int value = 1;
			int num = FindColInfoIdx(i, fromColInfoIdx);
			if (num != -1)
			{
				value = GetColInfo(num).OutlineLevel;
				value = ((!indent) ? (value - 1) : (value + 1));
				value = Math.Max(0, value);
				value = Math.Min(7, value);
				fromColInfoIdx = Math.Max(0, num - 1);
			}
			SetColumn(i, null, null, value, null, null);
		}
	}

	public ColumnInfoRecord FindColumnInfo(int columnIndex)
	{
		int count = records.Count;
		for (int i = 0; i < count; i++)
		{
			ColumnInfoRecord colInfo = GetColInfo(i);
			if (colInfo.ContainsColumn(columnIndex))
			{
				return colInfo;
			}
		}
		return null;
	}

	private int FindColInfoIdx(int columnIx, int fromColInfoIdx)
	{
		if (columnIx < 0)
		{
			throw new ArgumentException("column parameter out of range: " + columnIx);
		}
		if (fromColInfoIdx < 0)
		{
			throw new ArgumentException("fromIdx parameter out of range: " + fromColInfoIdx);
		}
		for (int i = fromColInfoIdx; i < records.Count; i++)
		{
			ColumnInfoRecord colInfo = GetColInfo(i);
			if (colInfo.ContainsColumn(columnIx))
			{
				return i;
			}
			if (colInfo.FirstColumn > columnIx)
			{
				break;
			}
		}
		return -1;
	}

	public int GetOutlineLevel(int columnIndex)
	{
		return FindColumnInfo(columnIndex)?.OutlineLevel ?? 0;
	}
}
