using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.Util.Collections;
using NPOI.XSSF.Util;

namespace NPOI.XSSF.UserModel.Helpers;

public class ColumnHelper
{
	public class TreeSet<T>
	{
		private SortedList<T, object> innerObj;

		private IComparer<T> comparer;

		public int Count => innerObj.Count;

		public TreeSet(IComparer<T> comparer)
		{
			this.comparer = comparer;
			innerObj = new SortedList<T, object>(comparer);
		}

		public T First()
		{
			IEnumerator<T> enumerator = innerObj.Keys.GetEnumerator();
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
			return default;
		}

		public T Higher(T element)
		{
			IEnumerator<T> enumerator = innerObj.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (innerObj.Comparer.Compare(enumerator.Current, element) > 0)
				{
					return enumerator.Current;
				}
			}
			return default;
		}

		public void Add(T item)
		{
			if (!innerObj.ContainsKey(item))
			{
				innerObj.Add(item, null);
			}
		}

		public bool Remove(T item)
		{
			return innerObj.Remove(item);
		}

		public void CopyTo(T[] target)
		{
			for (int i = 0; i < innerObj.Count; i++)
			{
				target[i] = innerObj.Keys[i];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return innerObj.Keys.GetEnumerator();
		}

		public T[] ToArray(T[] a)
		{
			List<T> list = new List<T>();
			list.AddRange(innerObj.Keys);
			if (a.Length < Count)
			{
				return list.ToArray();
			}
			Array.Copy(list.ToArray(), 0, a, 0, Count);
			if (a.Length > Count)
			{
				a[Count] = default;
			}
			return a;
		}

		internal void AddAll(List<T> list)
		{
			foreach (T item in list)
			{
				if (!innerObj.ContainsKey(item))
				{
					innerObj.Add(item, null);
				}
			}
		}

		internal void RemoveAll(List<T> list)
		{
			foreach (T item in list)
			{
				innerObj.Remove(item);
			}
		}

		internal T Lower(T element)
		{
			IEnumerator<T> enumerator = innerObj.Keys.GetEnumerator();
			T result = default;
			while (enumerator.MoveNext())
			{
				if (innerObj.Comparer.Compare(enumerator.Current, element) >= 0)
				{
					return result;
				}
				result = enumerator.Current;
			}
			return result;
		}

		internal TreeSet<T> TailSet(T fromElement, bool inclusive)
		{
			TreeSet<T> treeSet = new TreeSet<T>(comparer);
			IEnumerator<T> enumerator = innerObj.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (inclusive)
				{
					if (innerObj.Comparer.Compare(enumerator.Current, fromElement) >= 0)
					{
						treeSet.Add(enumerator.Current);
					}
				}
				else if (innerObj.Comparer.Compare(enumerator.Current, fromElement) > 0)
				{
					treeSet.Add(enumerator.Current);
				}
			}
			return treeSet;
		}
	}

	private CT_Worksheet worksheet;

	public ColumnHelper(CT_Worksheet worksheet)
	{
		this.worksheet = worksheet;
		CleanColumns();
	}

	public void CleanColumns()
	{
		TreeSet<CT_Col> treeSet = new TreeSet<CT_Col>(CTColComparator.BY_MIN_MAX);
		CT_Cols cT_Cols = new CT_Cols();
		CT_Cols[] array = worksheet.GetColsList().ToArray();
		int num = 0;
		for (num = 0; num < array.Length; num++)
		{
			CT_Col[] array2 = array[num].GetColList().ToArray();
			foreach (CT_Col newCol in array2)
			{
				AddCleanColIntoCols(cT_Cols, newCol, treeSet);
			}
		}
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			worksheet.RemoveCols(num2);
		}
		cT_Cols.SetColArray(treeSet.ToArray(new CT_Col[treeSet.Count]));
		worksheet.AddNewCols();
		worksheet.SetColsArray(0, cT_Cols);
	}

	public CT_Cols AddCleanColIntoCols(CT_Cols cols, CT_Col newCol)
	{
		TreeSet<CT_Col> treeSet = new TreeSet<CT_Col>(CTColComparator.BY_MIN_MAX);
		treeSet.AddAll(cols.GetColList());
		AddCleanColIntoCols(cols, newCol, treeSet);
		cols.SetColArray(treeSet.ToArray(new CT_Col[0]));
		return cols;
	}

	private void AddCleanColIntoCols(CT_Cols cols, CT_Col newCol, TreeSet<CT_Col> trackedCols)
	{
		List<CT_Col> overlappingCols = GetOverlappingCols(newCol, trackedCols);
		if (overlappingCols.Count == 0)
		{
			trackedCols.Add(CloneCol(cols, newCol));
			return;
		}
		trackedCols.RemoveAll(overlappingCols);
		foreach (CT_Col item in overlappingCols)
		{
			long[] overlap = GetOverlap(newCol, item);
			CT_Col cT_Col = CloneCol(cols, item, overlap);
			SetColumnAttributes(newCol, cT_Col);
			trackedCols.Add(cT_Col);
			CT_Col col = ((item.min < newCol.min) ? item : newCol);
			long[] array = new long[2]
			{
				Math.Min(item.min, newCol.min),
				overlap[0] - 1
			};
			if (array[0] <= array[1])
			{
				trackedCols.Add(CloneCol(cols, col, array));
			}
			CT_Col col2 = ((item.max > newCol.max) ? item : newCol);
			long[] array2 = new long[2]
			{
				overlap[1] + 1,
				Math.Max(item.max, newCol.max)
			};
			if (array2[0] <= array2[1])
			{
				trackedCols.Add(CloneCol(cols, col2, array2));
			}
		}
	}

	private CT_Col CloneCol(CT_Cols cols, CT_Col col, long[] newRange)
	{
		CT_Col cT_Col = CloneCol(cols, col);
		cT_Col.min = (uint)newRange[0];
		cT_Col.max = (uint)newRange[1];
		return cT_Col;
	}

	private long[] GetOverlap(CT_Col col1, CT_Col col2)
	{
		return GetOverlappingRange(col1, col2);
	}

	private List<CT_Col> GetOverlappingCols(CT_Col newCol, TreeSet<CT_Col> trackedCols)
	{
		CT_Col cT_Col = trackedCols.Lower(newCol);
		TreeSet<CT_Col> obj = ((cT_Col == null) ? trackedCols : trackedCols.TailSet(cT_Col, Overlaps(cT_Col, newCol)));
		List<CT_Col> list = new List<CT_Col>();
		foreach (CT_Col item in obj)
		{
			if (Overlaps(newCol, item))
			{
				list.Add(item);
				continue;
			}
			break;
		}
		return list;
	}

	private bool Overlaps(CT_Col col1, CT_Col col2)
	{
		return NumericRanges.GetOverlappingType(ToRange(col1), ToRange(col2)) != -1;
	}

	private long[] GetOverlappingRange(CT_Col col1, CT_Col col2)
	{
		return NumericRanges.GetOverlappingRange(ToRange(col1), ToRange(col2));
	}

	private long[] ToRange(CT_Col col)
	{
		return new long[2] { col.min, col.max };
	}

	public static void SortColumns(CT_Cols newCols)
	{
		List<CT_Col> colList = newCols.GetColList();
		colList.Sort(new CTColComparator());
		newCols.SetColArray(colList);
	}

	public CT_Col CloneCol(CT_Cols cols, CT_Col col)
	{
		CT_Col cT_Col = cols.AddNewCol();
		cT_Col.min = col.min;
		cT_Col.max = col.max;
		SetColumnAttributes(col, cT_Col);
		return cT_Col;
	}

	public CT_Col GetColumn(long index, bool splitColumns)
	{
		return GetColumn1Based(index + 1, splitColumns);
	}

	public CT_Col GetColumn1Based(long index1, bool splitColumns)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col[] array = colsArray.GetColList().ToArray();
		foreach (CT_Col cT_Col in array)
		{
			long num = cT_Col.min;
			long num2 = cT_Col.max;
			if (num > index1 || num2 < index1)
			{
				continue;
			}
			if (splitColumns)
			{
				if (num < index1)
				{
					InsertCol(colsArray, num, index1 - 1, new CT_Col[1] { cT_Col });
				}
				if (num2 > index1)
				{
					InsertCol(colsArray, index1 + 1, num2, new CT_Col[1] { cT_Col });
				}
				cT_Col.min = (uint)index1;
				cT_Col.max = (uint)index1;
			}
			return cT_Col;
		}
		return null;
	}

	private void SweepCleanColumns(CT_Cols cols, CT_Col[] flattenedColsArray, CT_Col overrideColumn)
	{
		List<CT_Col> list = new List<CT_Col>(flattenedColsArray);
		TreeSet<CT_Col> treeSet = new TreeSet<CT_Col>(CTColComparator.BY_MAX);
		list.GetEnumerator();
		CT_Col overrideColumn2 = null;
		long num = 0L;
		long num2 = 0L;
		IList<CT_Col> list2 = new List<CT_Col>();
		int num3 = -1;
		while (num3 + 1 < list.Count)
		{
			num3++;
			CT_Col cT_Col = list[num3];
			long num4 = cT_Col.min;
			long num5 = cT_Col.max;
			long num6 = ((num5 > num2) ? num5 : num2);
			if (num3 + 1 < list.Count)
			{
				num6 = list[num3 + 1].min;
			}
			IEnumerator<CT_Col> enumerator = treeSet.GetEnumerator();
			list2.Clear();
			while (enumerator.MoveNext())
			{
				CT_Col current = enumerator.Current;
				if (num4 <= current.max)
				{
					break;
				}
				list2.Add(current);
			}
			foreach (CT_Col item in list2)
			{
				treeSet.Remove(item);
			}
			if (treeSet.Count != 0 && num < num4)
			{
				CT_Col[] array = new CT_Col[treeSet.Count];
				treeSet.CopyTo(array);
				InsertCol(cols, num, num4 - 1, array, ignoreExistsCheck: true, overrideColumn2);
			}
			treeSet.Add(cT_Col);
			if (num5 > num2)
			{
				num2 = num5;
			}
			if (cT_Col.Equals(overrideColumn))
			{
				overrideColumn2 = overrideColumn;
			}
			while (num4 <= num6 && treeSet.Count != 0)
			{
				NPOI.Util.Collections.HashSet<CT_Col> hashSet = new NPOI.Util.Collections.HashSet<CT_Col>();
				CT_Col cT_Col2 = treeSet.First();
				long num7 = cT_Col2.max;
				hashSet.Add(cT_Col2);
				while (true)
				{
					CT_Col cT_Col3 = treeSet.Higher(cT_Col2);
					if (cT_Col3 == null || cT_Col3.max != num7)
					{
						break;
					}
					cT_Col2 = cT_Col3;
					hashSet.Add(cT_Col2);
					if (num5 > num2)
					{
						num2 = num5;
					}
					if (cT_Col.Equals(overrideColumn))
					{
						overrideColumn2 = overrideColumn;
					}
				}
				if (num7 < num6 || num3 + 1 >= list.Count)
				{
					CT_Col[] array2 = new CT_Col[treeSet.Count];
					treeSet.CopyTo(array2);
					InsertCol(cols, num4, num7, array2, ignoreExistsCheck: true, overrideColumn2);
					if (num3 + 1 < list.Count)
					{
						if (num6 > num7)
						{
							foreach (CT_Col item2 in hashSet)
							{
								treeSet.Remove(item2);
							}
							if (hashSet.Contains(overrideColumn))
							{
								overrideColumn2 = null;
							}
						}
					}
					else
					{
						foreach (CT_Col item3 in hashSet)
						{
							treeSet.Remove(item3);
						}
						if (hashSet.Contains(overrideColumn))
						{
							overrideColumn2 = null;
						}
					}
					num = (num4 = num7 + 1);
				}
				else
				{
					num = num4;
					num4 = num6 + 1;
				}
			}
		}
		SortColumns(cols);
	}

	private CT_Col InsertCol(CT_Cols cols, long min, long max, CT_Col[] colsWithAttributes)
	{
		return InsertCol(cols, min, max, colsWithAttributes, ignoreExistsCheck: false, null);
	}

	private CT_Col InsertCol(CT_Cols cols, long min, long max, CT_Col[] colsWithAttributes, bool ignoreExistsCheck, CT_Col overrideColumn)
	{
		if (ignoreExistsCheck || !ColumnExists(cols, min, max))
		{
			CT_Col cT_Col = cols.InsertNewCol(0);
			cT_Col.min = (uint)min;
			cT_Col.max = (uint)max;
			foreach (CT_Col fromCol in colsWithAttributes)
			{
				SetColumnAttributes(fromCol, cT_Col);
			}
			if (overrideColumn != null)
			{
				SetColumnAttributes(overrideColumn, cT_Col);
			}
			return cT_Col;
		}
		return null;
	}

	public bool ColumnExists(CT_Cols cols, long index)
	{
		return ColumnExists1Based(cols, index + 1);
	}

	private bool ColumnExists1Based(CT_Cols cols, long index1)
	{
		for (int i = 0; i < cols.sizeOfColArray(); i++)
		{
			if (cols.GetColArray(i).min == index1)
			{
				return true;
			}
		}
		return false;
	}

	public void SetColumnAttributes(CT_Col fromCol, CT_Col toCol)
	{
		if (fromCol.IsSetBestFit())
		{
			toCol.bestFit = fromCol.bestFit;
		}
		if (fromCol.IsSetCustomWidth())
		{
			toCol.customWidth = fromCol.customWidth;
		}
		if (fromCol.IsSetHidden())
		{
			toCol.hidden = fromCol.hidden;
		}
		if (fromCol.IsSetStyle())
		{
			toCol.style = fromCol.style;
		}
		if (fromCol.IsSetWidth())
		{
			toCol.width = fromCol.width;
			toCol.widthSpecified = fromCol.widthSpecified;
		}
		if (fromCol.IsSetCollapsed())
		{
			toCol.collapsed = fromCol.collapsed;
			toCol.collapsedSpecified = fromCol.collapsedSpecified;
		}
		if (fromCol.IsSetPhonetic())
		{
			toCol.phonetic = fromCol.phonetic;
		}
		if (fromCol.IsSetOutlineLevel())
		{
			toCol.outlineLevel = fromCol.outlineLevel;
		}
		if (fromCol.IsSetCollapsed())
		{
			toCol.collapsed = fromCol.collapsed;
		}
	}

	public void SetColBestFit(long index, bool bestFit)
	{
		GetOrCreateColumn1Based(index + 1, splitColumns: false).bestFit = bestFit;
	}

	public void SetCustomWidth(long index, bool width)
	{
		GetOrCreateColumn1Based(index + 1, splitColumns: true).customWidth = width;
	}

	public void SetColWidth(long index, double width)
	{
		GetOrCreateColumn1Based(index + 1, splitColumns: true).width = width;
	}

	public void SetColHidden(long index, bool hidden)
	{
		GetOrCreateColumn1Based(index + 1, splitColumns: true).hidden = hidden;
	}

	internal CT_Col GetOrCreateColumn1Based(long index1, bool splitColumns)
	{
		CT_Col cT_Col = GetColumn1Based(index1, splitColumns);
		if (cT_Col == null)
		{
			cT_Col = worksheet.GetColsArray(0).AddNewCol();
			cT_Col.min = (uint)index1;
			cT_Col.max = (uint)index1;
		}
		return cT_Col;
	}

	public void SetColDefaultStyle(long index, ICellStyle style)
	{
		SetColDefaultStyle(index, style.Index);
	}

	public void SetColDefaultStyle(long index, int styleId)
	{
		GetOrCreateColumn1Based(index + 1, splitColumns: true).style = (uint)styleId;
	}

	public int GetColDefaultStyle(long index)
	{
		if (GetColumn(index, splitColumns: false) != null)
		{
			return (int)GetColumn(index, splitColumns: false).style.Value;
		}
		return -1;
	}

	private bool ColumnExists(CT_Cols cols, long min, long max)
	{
		for (int i = 0; i < cols.sizeOfColArray(); i++)
		{
			if (cols.GetColArray(i).min == min && cols.GetColArray(i).max == max)
			{
				return true;
			}
		}
		return false;
	}

	public int GetIndexOfColumn(CT_Cols cols, CT_Col col)
	{
		for (int i = 0; i < cols.sizeOfColArray(); i++)
		{
			if (cols.GetColArray(i).min == col.min && cols.GetColArray(i).max == col.max)
			{
				return i;
			}
		}
		return -1;
	}
}
