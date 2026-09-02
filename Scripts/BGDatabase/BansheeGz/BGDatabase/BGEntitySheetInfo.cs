using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGEntitySheetInfo : BGSheetInfoA
{
	private readonly BGIdDictionary<int> fieldId2Column = new BGIdDictionary<int>();

	private readonly BGId metaId;

	private readonly string name;

	public string SheetName;

	private int indexId = -1;

	public BGId MetaId => metaId;

	public string Name => name;

	public int IndexId
	{
		get
		{
			return indexId;
		}
		set
		{
			if (indexId < 0)
			{
				indexId = value;
			}
		}
	}

	public bool HasAnyData
	{
		get
		{
			if (fieldId2Column.Count <= 0)
			{
				return HasId;
			}
			return true;
		}
	}

	public bool HasId => IndexId >= 0;

	public List<BGId> FieldIds => new List<BGId>(fieldId2Column.Keys);

	public List<BGId> EntityIds => new List<BGId>(Id2Row.Keys);

	public int PhysicalColumnCount { get; set; }

	public int PhysicalRowCount { get; set; }

	public int MaxColumn
	{
		get
		{
			int val = -1;
			val = Math.Max(val, indexId);
			foreach (KeyValuePair<BGId, int> item in fieldId2Column)
			{
				val = Math.Max(val, item.Value);
			}
			return val;
		}
	}

	public int FieldsCount => fieldId2Column.Count;

	public BGEntitySheetInfo(BGId metaId, string name, int sheetNumber)
		: base(sheetNumber)
	{
		this.metaId = metaId;
		this.name = name;
	}

	public override object Clone()
	{
		BGEntitySheetInfo bGEntitySheetInfo = new BGEntitySheetInfo(metaId, name, SheetNumber)
		{
			indexId = indexId,
			SheetName = SheetName
		};
		Clone(bGEntitySheetInfo);
		foreach (KeyValuePair<BGId, int> item in fieldId2Column)
		{
			bGEntitySheetInfo.fieldId2Column.Add(item.Key, item.Value);
		}
		return bGEntitySheetInfo;
	}

	public int GetFieldColumn(BGId fieldId)
	{
		if (!fieldId2Column.TryGetValue(fieldId, out var value))
		{
			return -1;
		}
		return value;
	}

	public bool HasField(BGId fieldId)
	{
		return fieldId2Column.ContainsKey(fieldId);
	}

	public void AddField(BGId fieldId, int columnIndex)
	{
		if (!HasField(fieldId))
		{
			fieldId2Column[fieldId] = columnIndex;
		}
	}

	public void ForEachField(Action<BGId, int> action)
	{
		foreach (KeyValuePair<BGId, int> item in fieldId2Column)
		{
			action(item.Key, item.Value);
		}
	}

	public void SetField(BGId fieldId, int column)
	{
		fieldId2Column[fieldId] = column;
	}

	public void SetEntity(BGId entityId, int row)
	{
		Id2Row[entityId] = row;
		Row2Id[row] = entityId;
	}

	public override void Clear()
	{
		base.Clear();
		fieldId2Column.Clear();
		indexId = -1;
	}

	public List<Tuple<BGField, int>> GetFieldsInfo(BGMetaEntity meta)
	{
		List<Tuple<BGField, int>> list = new List<Tuple<BGField, int>>();
		foreach (KeyValuePair<BGId, int> item in fieldId2Column)
		{
			list.Add(Tuple.Create(meta.GetField(item.Key), item.Value));
		}
		return list;
	}

	public static BGField[] GetFieldsArray(List<Tuple<BGField, int>> list)
	{
		BGField[] array = new BGField[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i].Item1;
		}
		return array;
	}
}
