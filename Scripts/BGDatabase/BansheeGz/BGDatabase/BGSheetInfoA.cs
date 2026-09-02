using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGSheetInfoA : ICloneable
{
	protected readonly BGIdDictionary<int> Id2Row = new BGIdDictionary<int>();

	protected readonly Dictionary<int, BGId> Row2Id = new Dictionary<int, BGId>();

	public readonly int SheetNumber;

	public int RowCount => Id2Row.Count;

	protected BGSheetInfoA(int sheetNumber)
	{
		SheetNumber = sheetNumber;
	}

	public virtual void Clear()
	{
		Id2Row.Clear();
		Row2Id.Clear();
	}

	public abstract object Clone();

	public void AddRow(BGId entityId, int rowIndex)
	{
		if (!HasRow(entityId))
		{
			Id2Row[entityId] = rowIndex;
			Row2Id[rowIndex] = entityId;
		}
	}

	public bool HasRow(BGId entityId)
	{
		return Id2Row.ContainsKey(entityId);
	}

	public int GetRow(BGId entityId)
	{
		if (!Id2Row.TryGetValue(entityId, out var value))
		{
			return -1;
		}
		return value;
	}

	public BGId GetRowId(int index)
	{
		if (!Row2Id.TryGetValue(index, out var value))
		{
			return BGId.Empty;
		}
		return value;
	}

	public void RemoveRow(BGId entityId)
	{
		if (Id2Row.TryGetValue(entityId, out var value))
		{
			Id2Row.Remove(entityId);
			Row2Id.Remove(value);
		}
	}

	protected void Clone(BGSheetInfoA to)
	{
		foreach (KeyValuePair<BGId, int> item in Id2Row)
		{
			to.Id2Row.Add(item.Key, item.Value);
		}
		foreach (KeyValuePair<int, BGId> item2 in Row2Id)
		{
			to.Row2Id.Add(item2.Key, item2.Value);
		}
	}

	public void ForEachRow(Action<BGId, int> action)
	{
		foreach (KeyValuePair<BGId, int> item in Id2Row)
		{
			action(item.Key, item.Value);
		}
	}
}
